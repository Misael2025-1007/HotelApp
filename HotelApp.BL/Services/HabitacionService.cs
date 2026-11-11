using HotelApp.BL.Base;
using HotelApp.BL.Dtos;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Enums;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.Logging;

namespace HotelApp.BL.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IHabitacionRepository _habitacionRepo;
        private readonly IPisoRepository _pisoRepo;
        private readonly ITipoHabitacionRepository _tipoRepo;
        private readonly ILogger<HabitacionService> _logger;

        public HabitacionService(IHabitacionRepository habitacionRepo, IPisoRepository pisoRepo,
            ITipoHabitacionRepository tipoRepo, ILogger<HabitacionService> logger)
        {
            _habitacionRepo = habitacionRepo;
            _pisoRepo = pisoRepo;
            _tipoRepo = tipoRepo;
            _logger = logger;
        }

        public Task<OperationResult> GetAllAsync() =>
            ServiceResult.TryAsync(async () =>
            {
                var habitaciones = await _habitacionRepo.GetAllConDetalleAsync();
                var dtos = habitaciones
                    .OrderBy(h => h.Piso.Hotel.Nombre).ThenBy(h => h.Piso.Numero).ThenBy(h => h.Numero)
                    .Select(ToDto).ToList();
                _logger.LogInformation("Consulta de habitaciones: {Total} registros", dtos.Count);
                return ServiceResult.Ok(data: dtos);
            }, _logger, "Ocurrió un error al obtener las habitaciones.");

        public Task<OperationResult> GetByIdAsync(int id) =>
            ServiceResult.TryAsync(async () =>
            {
                var habitacion = await _habitacionRepo.GetByIdAsync(id);
                if (habitacion == null)
                    return ServiceResult.Reject(_logger, "consultar habitación", "La habitación no existe.", id);

                return ServiceResult.Ok(data: new HabitacionDto
                {
                    Id = habitacion.Id,
                    PisoId = habitacion.PisoId,
                    TipoHabitacionId = habitacion.TipoHabitacionId,
                    Numero = habitacion.Numero,
                    Capacidad = habitacion.Capacidad,
                    Tarifa = habitacion.Tarifa,
                    Estado = habitacion.Estado,
                    Status = habitacion.Status,
                    CreationDate = habitacion.CreationDate,
                    ModifyDate = habitacion.ModifyDate
                });
            }, _logger, "Ocurrió un error al obtener la habitación.");

        public Task<OperationResult> SaveAsync(SaveHabitacionDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var error = await ValidarAsync(dto, null);
                if (error != null) return error;

                var habitacion = new Habitacion
                {
                    PisoId = dto.PisoId,
                    TipoHabitacionId = dto.TipoHabitacionId,
                    Numero = dto.Numero.Trim(),
                    Capacidad = dto.Capacidad,
                    Tarifa = dto.Tarifa,
                    Estado = dto.Estado,
                    UserMod = dto.UserId
                };
                await _habitacionRepo.AddAsync(habitacion);

                _logger.LogInformation("Habitación {HabitacionId} ({Numero}) registrada en el piso {PisoId}",
                    habitacion.Id, habitacion.Numero, habitacion.PisoId);
                return ServiceResult.Ok("Habitación registrada correctamente.", habitacion.Id);
            }, _logger, "Ocurrió un error al registrar la habitación.");

        public Task<OperationResult> UpdateAsync(UpdateHabitacionDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var habitacion = await _habitacionRepo.GetByIdAsync(dto.Id);
                if (habitacion == null)
                    return ServiceResult.Reject(_logger, "actualizar habitación", "La habitación no existe.", dto.Id);

                var error = await ValidarAsync(dto, dto.Id);
                if (error != null) return error;

                habitacion.PisoId = dto.PisoId;
                habitacion.TipoHabitacionId = dto.TipoHabitacionId;
                habitacion.Numero = dto.Numero.Trim();
                habitacion.Capacidad = dto.Capacidad;
                habitacion.Tarifa = dto.Tarifa;
                habitacion.Estado = dto.Estado;
                habitacion.UserMod = dto.UserId;
                await _habitacionRepo.UpdateAsync(habitacion);

                _logger.LogInformation("Habitación {HabitacionId} actualizada", habitacion.Id);
                return ServiceResult.Ok("Habitación actualizada correctamente.", habitacion.Id);
            }, _logger, "Ocurrió un error al actualizar la habitación.");

        public Task<OperationResult> RemoveAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var habitacion = await _habitacionRepo.GetByIdAsync(dto.Id);
                if (habitacion == null)
                    return ServiceResult.Reject(_logger, "desactivar habitación", "La habitación no existe.", dto.Id);

                if (!habitacion.Status)
                    return ServiceResult.Reject(_logger, "desactivar habitación", "La habitación ya está inactiva.", dto.Id);

                if (habitacion.Estado == EstadoHabitacion.Reserved || habitacion.Estado == EstadoHabitacion.Occupied)
                    return ServiceResult.Reject(_logger, "desactivar habitación",
                        "No se puede desactivar una habitación reservada u ocupada.", dto.Id);

                habitacion.Status = false;
                habitacion.UserMod = dto.UserId;
                await _habitacionRepo.UpdateAsync(habitacion);

                _logger.LogInformation("Habitación {HabitacionId} desactivada", habitacion.Id);
                return ServiceResult.Ok("Habitación desactivada correctamente.");
            }, _logger, "Ocurrió un error al desactivar la habitación.");

        public Task<OperationResult> ActivateAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var habitacion = await _habitacionRepo.GetByIdAsync(dto.Id);
                if (habitacion == null)
                    return ServiceResult.Reject(_logger, "activar habitación", "La habitación no existe.", dto.Id);

                if (habitacion.Status)
                    return ServiceResult.Reject(_logger, "activar habitación", "La habitación ya está activa.", dto.Id);

                var piso = await _pisoRepo.GetByIdAsync(habitacion.PisoId);
                if (piso == null || !piso.Status)
                    return ServiceResult.Reject(_logger, "activar habitación",
                        "No se puede activar una habitación de un piso inactivo.", dto.Id);

                // RN05
                if (await _habitacionRepo.ExisteNumeroEnHotelAsync(piso.HotelId, habitacion.Numero, habitacion.Id))
                    return ServiceResult.Reject(_logger, "activar habitación",
                        "Ya existe una habitación activa con ese número en el hotel.", dto.Id);

                habitacion.Status = true;
                habitacion.UserMod = dto.UserId;
                await _habitacionRepo.UpdateAsync(habitacion);

                _logger.LogInformation("Habitación {HabitacionId} activada", habitacion.Id);
                return ServiceResult.Ok("Habitación activada correctamente.");
            }, _logger, "Ocurrió un error al activar la habitación.");

        private async Task<OperationResult?> ValidarAsync(SaveHabitacionDto dto, int? excluirId)
        {
            if (string.IsNullOrWhiteSpace(dto.Numero))
                return ServiceResult.Reject(_logger, "guardar habitación", "El número de habitación es obligatorio.", excluirId);

            var piso = await _pisoRepo.GetByIdAsync(dto.PisoId);
            if (piso == null)
                return ServiceResult.Reject(_logger, "guardar habitación", "El piso seleccionado no existe.", excluirId);

            // RN06: un piso inactivo no puede recibir habitaciones
            if (!piso.Status)
                return ServiceResult.Reject(_logger, "guardar habitación",
                    "Un piso inactivo no puede recibir habitaciones.", excluirId);

            var tipo = await _tipoRepo.GetByIdAsync(dto.TipoHabitacionId);
            if (tipo == null || !tipo.Status)
                return ServiceResult.Reject(_logger, "guardar habitación",
                    "El tipo de habitación seleccionado no existe o está inactivo.", excluirId);

            if (!Enum.IsDefined(typeof(EstadoHabitacion), dto.Estado))
                return ServiceResult.Reject(_logger, "guardar habitación", "El estado de la habitación no es válido.", excluirId);

            // RN05: el número de habitación debe ser único dentro del hotel
            if (await _habitacionRepo.ExisteNumeroEnHotelAsync(piso.HotelId, dto.Numero.Trim(), excluirId))
                return ServiceResult.Reject(_logger, "guardar habitación",
                    "Ya existe una habitación activa con ese número en el hotel.", excluirId);

            return null;
        }

        private static HabitacionDto ToDto(Habitacion h) => new HabitacionDto
        {
            Id = h.Id,
            PisoId = h.PisoId,
            PisoNumero = h.Piso.Numero,
            HotelId = h.Piso.HotelId,
            HotelNombre = h.Piso.Hotel.Nombre,
            TipoHabitacionId = h.TipoHabitacionId,
            TipoNombre = h.TipoHabitacion.Nombre,
            Numero = h.Numero,
            Capacidad = h.Capacidad,
            Tarifa = h.Tarifa,
            Estado = h.Estado,
            Status = h.Status,
            CreationDate = h.CreationDate,
            ModifyDate = h.ModifyDate
        };
    }
}
