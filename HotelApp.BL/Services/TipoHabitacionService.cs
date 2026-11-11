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
    public class TipoHabitacionService : ITipoHabitacionService
    {
        private readonly ITipoHabitacionRepository _tipoRepo;
        private readonly IHabitacionRepository _habitacionRepo;
        private readonly ILogger<TipoHabitacionService> _logger;

        public TipoHabitacionService(ITipoHabitacionRepository tipoRepo,
            IHabitacionRepository habitacionRepo, ILogger<TipoHabitacionService> logger)
        {
            _tipoRepo = tipoRepo;
            _habitacionRepo = habitacionRepo;
            _logger = logger;
        }

        public Task<OperationResult> GetAllAsync() =>
            ServiceResult.TryAsync(async () =>
            {
                var tipos = await _tipoRepo.GetAllAsync();
                var dtos = tipos.OrderBy(t => t.Nombre).Select(ToDto).ToList();
                _logger.LogInformation("Consulta de tipos de habitación: {Total} registros", dtos.Count);
                return ServiceResult.Ok(data: dtos);
            }, _logger, "Ocurrió un error al obtener los tipos de habitación.");

        public Task<OperationResult> GetByIdAsync(int id) =>
            ServiceResult.TryAsync(async () =>
            {
                var tipo = await _tipoRepo.GetByIdAsync(id);
                if (tipo == null)
                    return ServiceResult.Reject(_logger, "consultar tipo de habitación", "El tipo de habitación no existe.", id);

                return ServiceResult.Ok(data: ToDto(tipo));
            }, _logger, "Ocurrió un error al obtener el tipo de habitación.");

        public Task<OperationResult> SaveAsync(SaveTipoHabitacionDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var error = await ValidarAsync(dto, null);
                if (error != null) return error;

                var tipo = new TipoHabitacion
                {
                    Nombre = dto.Nombre.Trim(),
                    Descripcion = dto.Descripcion?.Trim(),
                    UserMod = dto.UserId
                };
                await _tipoRepo.AddAsync(tipo);

                _logger.LogInformation("Tipo de habitación {TipoId} registrado: {Nombre}", tipo.Id, tipo.Nombre);
                return ServiceResult.Ok("Tipo de habitación registrado correctamente.", ToDto(tipo));
            }, _logger, "Ocurrió un error al registrar el tipo de habitación.");

        public Task<OperationResult> UpdateAsync(UpdateTipoHabitacionDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var tipo = await _tipoRepo.GetByIdAsync(dto.Id);
                if (tipo == null)
                    return ServiceResult.Reject(_logger, "actualizar tipo de habitación", "El tipo de habitación no existe.", dto.Id);

                var error = await ValidarAsync(dto, dto.Id);
                if (error != null) return error;

                tipo.Nombre = dto.Nombre.Trim();
                tipo.Descripcion = dto.Descripcion?.Trim();
                tipo.UserMod = dto.UserId;
                await _tipoRepo.UpdateAsync(tipo);

                _logger.LogInformation("Tipo de habitación {TipoId} actualizado", tipo.Id);
                return ServiceResult.Ok("Tipo de habitación actualizado correctamente.", ToDto(tipo));
            }, _logger, "Ocurrió un error al actualizar el tipo de habitación.");

        public Task<OperationResult> RemoveAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var tipo = await _tipoRepo.GetByIdAsync(dto.Id);
                if (tipo == null)
                    return ServiceResult.Reject(_logger, "desactivar tipo de habitación", "El tipo de habitación no existe.", dto.Id);

                if (!tipo.Status)
                    return ServiceResult.Reject(_logger, "desactivar tipo de habitación", "El tipo de habitación ya está inactivo.", dto.Id);

                if (await _habitacionRepo.ExistsAsync(h => h.TipoHabitacionId == dto.Id && h.Status))
                    return ServiceResult.Reject(_logger, "desactivar tipo de habitación",
                        "No se puede desactivar un tipo de habitación con habitaciones activas.", dto.Id);

                tipo.Status = false;
                tipo.UserMod = dto.UserId;
                await _tipoRepo.UpdateAsync(tipo);

                _logger.LogInformation("Tipo de habitación {TipoId} desactivado", tipo.Id);
                return ServiceResult.Ok("Tipo de habitación desactivado correctamente.");
            }, _logger, "Ocurrió un error al desactivar el tipo de habitación.");

        public Task<OperationResult> ActivateAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var tipo = await _tipoRepo.GetByIdAsync(dto.Id);
                if (tipo == null)
                    return ServiceResult.Reject(_logger, "activar tipo de habitación", "El tipo de habitación no existe.", dto.Id);

                if (tipo.Status)
                    return ServiceResult.Reject(_logger, "activar tipo de habitación", "El tipo de habitación ya está activo.", dto.Id);

                if (await _tipoRepo.ExisteNombreAsync(tipo.Nombre, tipo.Id))
                    return ServiceResult.Reject(_logger, "activar tipo de habitación",
                        "Ya existe un tipo de habitación activo con ese nombre.", dto.Id);

                tipo.Status = true;
                tipo.UserMod = dto.UserId;
                await _tipoRepo.UpdateAsync(tipo);

                _logger.LogInformation("Tipo de habitación {TipoId} activado", tipo.Id);
                return ServiceResult.Ok("Tipo de habitación activado correctamente.");
            }, _logger, "Ocurrió un error al activar el tipo de habitación.");

        private async Task<OperationResult?> ValidarAsync(SaveTipoHabitacionDto dto, int? excluirId)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return ServiceResult.Reject(_logger, "guardar tipo de habitación", "El nombre es obligatorio.", excluirId);

            if (await _tipoRepo.ExisteNombreAsync(dto.Nombre.Trim(), excluirId))
                return ServiceResult.Reject(_logger, "guardar tipo de habitación",
                    "Ya existe un tipo de habitación activo con ese nombre.", excluirId);

            return null;
        }

        private static TipoHabitacionDto ToDto(TipoHabitacion t) => new TipoHabitacionDto
        {
            Id = t.Id,
            Nombre = t.Nombre,
            Descripcion = t.Descripcion,
            Status = t.Status,
            CreationDate = t.CreationDate,
            ModifyDate = t.ModifyDate
        };
    }
}