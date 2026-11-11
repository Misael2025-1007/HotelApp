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
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepo;
        private readonly IPisoRepository _pisoRepo;
        private readonly IBloqueRepository _bloqueRepo;
        private readonly ILogger<HotelService> _logger;

        public HotelService(IHotelRepository hotelRepo, IPisoRepository pisoRepo,
            IBloqueRepository bloqueRepo, ILogger<HotelService> logger)
        {
            _hotelRepo = hotelRepo;
            _pisoRepo = pisoRepo;
            _bloqueRepo = bloqueRepo;
            _logger = logger;
        }

        public Task<OperationResult> GetAllAsync() =>
            ServiceResult.TryAsync(async () =>
            {
                var hoteles = await _hotelRepo.GetAllAsync();
                var dtos = hoteles.OrderBy(h => h.Nombre).Select(ToDto).ToList();
                _logger.LogInformation("Consulta de hoteles: {Total} registros", dtos.Count);
                return ServiceResult.Ok(data: dtos);
            }, _logger, "Ocurrió un error al obtener los hoteles.");

        public Task<OperationResult> GetByIdAsync(int id) =>
            ServiceResult.TryAsync(async () =>
            {
                var hotel = await _hotelRepo.GetByIdAsync(id);
                if (hotel == null)
                    return Rechazar("consultar hotel", "El hotel no existe.", id);

                return ServiceResult.Ok(data: ToDto(hotel));
            }, _logger, "Ocurrió un error al obtener el hotel.");

        public Task<OperationResult> SaveAsync(SaveHotelDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var error = await ValidarAsync(dto, null);
                if (error != null) return error;

                var hotel = new Hotel
                {
                    Nombre = dto.Nombre.Trim(),
                    Direccion = dto.Direccion.Trim(),
                    Telefono = dto.Telefono?.Trim(),
                    UserMod = dto.UserId
                };
                await _hotelRepo.AddAsync(hotel);

                _logger.LogInformation("Hotel {HotelId} registrado: {Nombre}", hotel.Id, hotel.Nombre);
                return ServiceResult.Ok("Hotel registrado correctamente.", ToDto(hotel));
            }, _logger, "Ocurrió un error al registrar el hotel.");

        public Task<OperationResult> UpdateAsync(UpdateHotelDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var hotel = await _hotelRepo.GetByIdAsync(dto.Id);
                if (hotel == null)
                    return Rechazar("actualizar hotel", "El hotel no existe.", dto.Id);

                var error = await ValidarAsync(dto, dto.Id);
                if (error != null) return error;

                hotel.Nombre = dto.Nombre.Trim();
                hotel.Direccion = dto.Direccion.Trim();
                hotel.Telefono = dto.Telefono?.Trim();
                hotel.UserMod = dto.UserId;
                await _hotelRepo.UpdateAsync(hotel);

                _logger.LogInformation("Hotel {HotelId} actualizado", hotel.Id);
                return ServiceResult.Ok("Hotel actualizado correctamente.", ToDto(hotel));
            }, _logger, "Ocurrió un error al actualizar el hotel.");

        public Task<OperationResult> RemoveAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var hotel = await _hotelRepo.GetByIdAsync(dto.Id);
                if (hotel == null)
                    return Rechazar("desactivar hotel", "El hotel no existe.", dto.Id);

                if (!hotel.Status)
                    return Rechazar("desactivar hotel", "El hotel ya está inactivo.", dto.Id);

                if (await _pisoRepo.ExistsAsync(p => p.HotelId == dto.Id && p.Status))
                    return Rechazar("desactivar hotel", "No se puede desactivar un hotel con pisos activos.", dto.Id);

                if (await _bloqueRepo.ExistsAsync(b => b.HotelId == dto.Id && b.Status))
                    return Rechazar("desactivar hotel", "No se puede desactivar un hotel con bloques activos.", dto.Id);

                hotel.Status = false;
                hotel.UserMod = dto.UserId;
                await _hotelRepo.UpdateAsync(hotel);

                _logger.LogInformation("Hotel {HotelId} desactivado", hotel.Id);
                return ServiceResult.Ok("Hotel desactivado correctamente.");
            }, _logger, "Ocurrió un error al desactivar el hotel.");

        public Task<OperationResult> ActivateAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var hotel = await _hotelRepo.GetByIdAsync(dto.Id);
                if (hotel == null)
                    return Rechazar("activar hotel", "El hotel no existe.", dto.Id);

                if (hotel.Status)
                    return Rechazar("activar hotel", "El hotel ya está activo.", dto.Id);

                if (await _hotelRepo.ExisteNombreAsync(hotel.Nombre, hotel.Id))
                    return Rechazar("activar hotel", "Ya existe un hotel activo con ese nombre.", dto.Id);

                hotel.Status = true;
                hotel.UserMod = dto.UserId;
                await _hotelRepo.UpdateAsync(hotel);

                _logger.LogInformation("Hotel {HotelId} activado", hotel.Id);
                return ServiceResult.Ok("Hotel activado correctamente.");
            }, _logger, "Ocurrió un error al activar el hotel.");

        // ---------- Validaciones de negocio ----------
        private async Task<OperationResult?> ValidarAsync(SaveHotelDto dto, int? excluirId)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Rechazar("guardar hotel", "El nombre es obligatorio.", excluirId);

            if (string.IsNullOrWhiteSpace(dto.Direccion))
                return Rechazar("guardar hotel", "La dirección es obligatoria.", excluirId);

            if (await _hotelRepo.ExisteNombreAsync(dto.Nombre.Trim(), excluirId))
                return Rechazar("guardar hotel", "Ya existe un hotel activo con ese nombre.", excluirId);

            return null;
        }

        // Registra el rechazo de negocio en el log y devuelve el error al llamador
        private OperationResult Rechazar(string operacion, string motivo, int? id)
        {
            _logger.LogWarning("Validación fallida en {Operacion} (Id: {Id}): {Motivo}", operacion, id, motivo);
            return ServiceResult.Fail(motivo);
        }

        private static HotelDto ToDto(Hotel h) => new HotelDto
        {
            Id = h.Id,
            Nombre = h.Nombre,
            Direccion = h.Direccion,
            Telefono = h.Telefono,
            Status = h.Status,
            CreationDate = h.CreationDate,
            ModifyDate = h.ModifyDate
        };
    }
}