using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.BL.Base;
using HotelApp.BL.Dtos;
using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.Logging;

namespace HotelApp.BL.Services
{
    public class BloqueService : IBloqueService
    {
        private readonly IBloqueRepository _bloqueRepo;
        private readonly IHotelRepository _hotelRepo;
        private readonly IPisoRepository _pisoRepo;
        private readonly ILogger<BloqueService> _logger;

        public BloqueService(IBloqueRepository bloqueRepo, IHotelRepository hotelRepo,
            IPisoRepository pisoRepo, ILogger<BloqueService> logger)
        {
            _bloqueRepo = bloqueRepo;
            _hotelRepo = hotelRepo;
            _pisoRepo = pisoRepo;
            _logger = logger;
        }

        public Task<OperationResult> GetAllAsync() =>
            ServiceResult.TryAsync(async () =>
            {
                var bloques = await _bloqueRepo.GetAllConHotelAsync();
                var dtos = bloques
                    .OrderBy(b => b.Hotel.Nombre).ThenBy(b => b.Nombre)
                    .Select(b => ToDto(b, b.Hotel.Nombre)).ToList();
                _logger.LogInformation("Consulta de bloques: {Total} registros", dtos.Count);
                return ServiceResult.Ok(data: dtos);
            }, _logger, "Ocurrió un error al obtener los bloques.");

        public Task<OperationResult> GetByIdAsync(int id) =>
            ServiceResult.TryAsync(async () =>
            {
                var bloque = await _bloqueRepo.GetByIdAsync(id);
                if (bloque == null)
                    return ServiceResult.Reject(_logger, "consultar bloque", "El bloque no existe.", id);

                var hotel = await _hotelRepo.GetByIdAsync(bloque.HotelId);
                return ServiceResult.Ok(data: ToDto(bloque, hotel?.Nombre ?? string.Empty));
            }, _logger, "Ocurrió un error al obtener el bloque.");

        public Task<OperationResult> SaveAsync(SaveBloqueDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var (error, hotel) = await ValidarAsync(dto, null);
                if (error != null) return error;

                var bloque = new Bloque
                {
                    HotelId = dto.HotelId,
                    Nombre = dto.Nombre.Trim(),
                    Descripcion = dto.Descripcion?.Trim(),
                    UserMod = dto.UserId
                };
                await _bloqueRepo.AddAsync(bloque);

                _logger.LogInformation("Bloque {BloqueId} registrado en el hotel {HotelId}", bloque.Id, bloque.HotelId);
                return ServiceResult.Ok("Bloque registrado correctamente.", ToDto(bloque, hotel!.Nombre));
            }, _logger, "Ocurrió un error al registrar el bloque.");

        public Task<OperationResult> UpdateAsync(UpdateBloqueDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var bloque = await _bloqueRepo.GetByIdAsync(dto.Id);
                if (bloque == null)
                    return ServiceResult.Reject(_logger, "actualizar bloque", "El bloque no existe.", dto.Id);

                var (error, hotel) = await ValidarAsync(dto, dto.Id);
                if (error != null) return error;

                // Un bloque con pisos no puede cambiar de hotel (los pisos quedarían en otro hotel)
                if (bloque.HotelId != dto.HotelId && await _pisoRepo.ExistsAsync(p => p.BloqueId == bloque.Id))
                    return ServiceResult.Reject(_logger, "actualizar bloque",
                        "No se puede cambiar el hotel de un bloque que tiene pisos asociados.", dto.Id);

                bloque.HotelId = dto.HotelId;
                bloque.Nombre = dto.Nombre.Trim();
                bloque.Descripcion = dto.Descripcion?.Trim();
                bloque.UserMod = dto.UserId;
                await _bloqueRepo.UpdateAsync(bloque);

                _logger.LogInformation("Bloque {BloqueId} actualizado", bloque.Id);
                return ServiceResult.Ok("Bloque actualizado correctamente.", ToDto(bloque, hotel!.Nombre));
            }, _logger, "Ocurrió un error al actualizar el bloque.");

        public Task<OperationResult> RemoveAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var bloque = await _bloqueRepo.GetByIdAsync(dto.Id);
                if (bloque == null)
                    return ServiceResult.Reject(_logger, "desactivar bloque", "El bloque no existe.", dto.Id);

                if (!bloque.Status)
                    return ServiceResult.Reject(_logger, "desactivar bloque", "El bloque ya está inactivo.", dto.Id);

                if (await _pisoRepo.ExistsAsync(p => p.BloqueId == dto.Id && p.Status))
                    return ServiceResult.Reject(_logger, "desactivar bloque",
                        "No se puede desactivar un bloque con pisos activos.", dto.Id);

                bloque.Status = false;
                bloque.UserMod = dto.UserId;
                await _bloqueRepo.UpdateAsync(bloque);

                _logger.LogInformation("Bloque {BloqueId} desactivado", bloque.Id);
                return ServiceResult.Ok("Bloque desactivado correctamente.");
            }, _logger, "Ocurrió un error al desactivar el bloque.");

        public Task<OperationResult> ActivateAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var bloque = await _bloqueRepo.GetByIdAsync(dto.Id);
                if (bloque == null)
                    return ServiceResult.Reject(_logger, "activar bloque", "El bloque no existe.", dto.Id);

                if (bloque.Status)
                    return ServiceResult.Reject(_logger, "activar bloque", "El bloque ya está activo.", dto.Id);

                var hotel = await _hotelRepo.GetByIdAsync(bloque.HotelId);
                if (hotel == null || !hotel.Status)
                    return ServiceResult.Reject(_logger, "activar bloque",
                        "No se puede activar un bloque de un hotel inactivo.", dto.Id);

                if (await _bloqueRepo.ExisteNombreEnHotelAsync(bloque.HotelId, bloque.Nombre, bloque.Id))
                    return ServiceResult.Reject(_logger, "activar bloque",
                        "Ya existe un bloque activo con ese nombre en el hotel.", dto.Id);

                bloque.Status = true;
                bloque.UserMod = dto.UserId;
                await _bloqueRepo.UpdateAsync(bloque);

                _logger.LogInformation("Bloque {BloqueId} activado", bloque.Id);
                return ServiceResult.Ok("Bloque activado correctamente.");
            }, _logger, "Ocurrió un error al activar el bloque.");

        private async Task<(OperationResult? Error, Hotel? Hotel)> ValidarAsync(SaveBloqueDto dto, int? excluirId)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return (ServiceResult.Reject(_logger, "guardar bloque", "El nombre es obligatorio.", excluirId), null);

            var hotel = await _hotelRepo.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                return (ServiceResult.Reject(_logger, "guardar bloque", "El hotel seleccionado no existe.", excluirId), null);

            if (!hotel.Status)
                return (ServiceResult.Reject(_logger, "guardar bloque", "El hotel seleccionado está inactivo.", excluirId), null);

            if (await _bloqueRepo.ExisteNombreEnHotelAsync(dto.HotelId, dto.Nombre.Trim(), excluirId))
                return (ServiceResult.Reject(_logger, "guardar bloque",
                    "Ya existe un bloque activo con ese nombre en el hotel.", excluirId), null);

            return (null, hotel);
        }

        private static BloqueDto ToDto(Bloque b, string hotelNombre) => new BloqueDto
        {
            Id = b.Id,
            HotelId = b.HotelId,
            HotelNombre = hotelNombre,
            Nombre = b.Nombre,
            Descripcion = b.Descripcion,
            Status = b.Status,
            CreationDate = b.CreationDate,
            ModifyDate = b.ModifyDate
        };
    }
}