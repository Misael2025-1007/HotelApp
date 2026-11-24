using HotelApp.BL.Base;
using HotelApp.BL.Dtos;
using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.Logging;

namespace HotelApp.BL.Services
{
    public class PisoService : IPisoService
    {
        private readonly IPisoRepository _pisoRepo;
        private readonly IHotelRepository _hotelRepo;
        private readonly IBloqueRepository _bloqueRepo;
        private readonly IHabitacionRepository _habitacionRepo;
        private readonly ILogger<PisoService> _logger;

        public PisoService(IPisoRepository pisoRepo, IHotelRepository hotelRepo,
            IBloqueRepository bloqueRepo, IHabitacionRepository habitacionRepo, ILogger<PisoService> logger)
        {
            _pisoRepo = pisoRepo;
            _hotelRepo = hotelRepo;
            _bloqueRepo = bloqueRepo;
            _habitacionRepo = habitacionRepo;
            _logger = logger;
        }

        public Task<OperationResult> GetAllAsync() =>
            ServiceResult.TryAsync(async () =>
            {
                var pisos = await _pisoRepo.GetAllConDetalleAsync();
                var dtos = pisos.OrderBy(p => p.Hotel.Nombre).ThenBy(p => p.Numero)
                    .Select(p => ToDto(p, p.Hotel.Nombre, p.Bloque?.Nombre)).ToList();
                _logger.LogInformation("Consulta de pisos: {Total} registros", dtos.Count);
                return ServiceResult.Ok(data: dtos);
            }, _logger, "Ocurrió un error al obtener los pisos.");

        public Task<OperationResult> GetByIdAsync(int id) =>
            ServiceResult.TryAsync(async () =>
            {
                var piso = await _pisoRepo.GetByIdAsync(id);
                if (piso == null)
                    return ServiceResult.Reject(_logger, "consultar piso", "El piso no existe.", id);

                var hotel = await _hotelRepo.GetByIdAsync(piso.HotelId);
                var bloque = piso.BloqueId.HasValue ? await _bloqueRepo.GetByIdAsync(piso.BloqueId.Value) : null;
                return ServiceResult.Ok(data: ToDto(piso, hotel?.Nombre ?? string.Empty, bloque?.Nombre));
            }, _logger, "Ocurrió un error al obtener el piso.");

        public Task<OperationResult> SaveAsync(SavePisoDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var error = await ValidarAsync(dto, null);
                if (error != null) return error;

                var piso = new Piso
                {
                    HotelId = dto.HotelId,
                    BloqueId = dto.BloqueId,
                    Numero = dto.Numero,
                    UserMod = dto.UserId
                };
                await _pisoRepo.AddAsync(piso);

                _logger.LogInformation("Piso {PisoId} (número {Numero}) registrado en el hotel {HotelId}",
                    piso.Id, piso.Numero, piso.HotelId);
                return ServiceResult.Ok("Piso registrado correctamente.", await ToDtoAsync(piso));
            }, _logger, "Ocurrió un error al registrar el piso.");

        public Task<OperationResult> UpdateAsync(UpdatePisoDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var piso = await _pisoRepo.GetByIdAsync(dto.Id);
                if (piso == null)
                    return ServiceResult.Reject(_logger, "actualizar piso", "El piso no existe.", dto.Id);

                var error = await ValidarAsync(dto, dto.Id);
                if (error != null) return error;

                if (piso.HotelId != dto.HotelId && await _habitacionRepo.ExistsAsync(h => h.PisoId == piso.Id))
                    return ServiceResult.Reject(_logger, "actualizar piso",
                        "No se puede cambiar el hotel de un piso que tiene habitaciones.", dto.Id);

                piso.HotelId = dto.HotelId;
                piso.BloqueId = dto.BloqueId;
                piso.Numero = dto.Numero;
                piso.UserMod = dto.UserId;
                await _pisoRepo.UpdateAsync(piso);

                _logger.LogInformation("Piso {PisoId} actualizado", piso.Id);
                return ServiceResult.Ok("Piso actualizado correctamente.", await ToDtoAsync(piso));
            }, _logger, "Ocurrió un error al actualizar el piso.");

        public Task<OperationResult> RemoveAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var piso = await _pisoRepo.GetByIdAsync(dto.Id);
                if (piso == null)
                    return ServiceResult.Reject(_logger, "desactivar piso", "El piso no existe.", dto.Id);

                if (!piso.Status)
                    return ServiceResult.Reject(_logger, "desactivar piso", "El piso ya está inactivo.", dto.Id);

                if (await _habitacionRepo.ExistsAsync(h => h.PisoId == dto.Id && h.Status))
                    return ServiceResult.Reject(_logger, "desactivar piso",
                        "No se puede desactivar un piso con habitaciones activas.", dto.Id);

                piso.Status = false;
                piso.UserMod = dto.UserId;
                await _pisoRepo.UpdateAsync(piso);

                _logger.LogInformation("Piso {PisoId} desactivado", piso.Id);
                return ServiceResult.Ok("Piso desactivado correctamente.");
            }, _logger, "Ocurrió un error al desactivar el piso.");

        public Task<OperationResult> ActivateAsync(RemoveDto dto) =>
            ServiceResult.TryAsync(async () =>
            {
                var piso = await _pisoRepo.GetByIdAsync(dto.Id);
                if (piso == null)
                    return ServiceResult.Reject(_logger, "activar piso", "El piso no existe.", dto.Id);

                if (piso.Status)
                    return ServiceResult.Reject(_logger, "activar piso", "El piso ya está activo.", dto.Id);

                var hotel = await _hotelRepo.GetByIdAsync(piso.HotelId);
                if (hotel == null || !hotel.Status)
                    return ServiceResult.Reject(_logger, "activar piso",
                        "No se puede activar un piso de un hotel inactivo.", dto.Id);

                // RN03
                if (await _pisoRepo.ExisteNumeroActivoAsync(piso.HotelId, piso.Numero, piso.Id))
                    return ServiceResult.Reject(_logger, "activar piso",
                        $"Ya existe un piso activo con el número {piso.Numero} en este hotel.", dto.Id);

                piso.Status = true;
                piso.UserMod = dto.UserId;
                await _pisoRepo.UpdateAsync(piso);

                _logger.LogInformation("Piso {PisoId} activado", piso.Id);
                return ServiceResult.Ok("Piso activado correctamente.");
            }, _logger, "Ocurrió un error al activar el piso.");

        private async Task<OperationResult?> ValidarAsync(SavePisoDto dto, int? excluirId)
        {
            var hotel = await _hotelRepo.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                return ServiceResult.Reject(_logger, "guardar piso", "El hotel seleccionado no existe.", excluirId);

            if (!hotel.Status)
                return ServiceResult.Reject(_logger, "guardar piso", "El hotel seleccionado está inactivo.", excluirId);

            if (dto.BloqueId.HasValue)
            {
                var bloque = await _bloqueRepo.GetByIdAsync(dto.BloqueId.Value);
                if (bloque == null || !bloque.Status)
                    return ServiceResult.Reject(_logger, "guardar piso",
                        "El bloque seleccionado no existe o está inactivo.", excluirId);

                if (bloque.HotelId != dto.HotelId)
                    return ServiceResult.Reject(_logger, "guardar piso",
                        "El bloque no pertenece al hotel seleccionado.", excluirId);
            }

            // RN03: no pueden existir dos pisos activos con el mismo número en un hotel
            if (await _pisoRepo.ExisteNumeroActivoAsync(dto.HotelId, dto.Numero, excluirId))
                return ServiceResult.Reject(_logger, "guardar piso",
                    $"Ya existe un piso activo con el número {dto.Numero} en este hotel.", excluirId);

            return null;
        }

        private async Task<PisoDto> ToDtoAsync(Piso p)
        {
            var hotel = await _hotelRepo.GetByIdAsync(p.HotelId);
            var bloque = p.BloqueId.HasValue ? await _bloqueRepo.GetByIdAsync(p.BloqueId.Value) : null;
            return ToDto(p, hotel?.Nombre ?? string.Empty, bloque?.Nombre);
        }

        private static PisoDto ToDto(Piso p, string hotelNombre, string? bloqueNombre) => new PisoDto
        {
            Id = p.Id,
            HotelId = p.HotelId,
            HotelNombre = hotelNombre,
            BloqueId = p.BloqueId,
            BloqueNombre = bloqueNombre,
            Numero = p.Numero,
            Status = p.Status,
            CreationDate = p.CreationDate,
            ModifyDate = p.ModifyDate
        };
    }
}
