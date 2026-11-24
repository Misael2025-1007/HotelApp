using HotelApp.BL.Base;
using HotelApp.BL.Dtos;
using HotelApp.BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    /// <summary>Gestión de bloques de un hotel.</summary>
    [ApiController]
    [Route("api/bloques")]
    [Produces("application/json")]
    public class BloquesController : ApiControllerBase
    {
        private readonly IBloqueService _service;

        public BloquesController(IBloqueService service) => _service = service;

        /// <summary>Lista todos los registros (activos e inactivos).</summary>
        /// <response code="200">Listado obtenido correctamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Respond(await _service.GetAllAsync());

        /// <summary>Obtiene un registro por su identificador.</summary>
        /// <param name="id">Identificador del registro.</param>
        /// <response code="200">Registro encontrado.</response>
        /// <response code="404">El registro no existe.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
            => Respond(await _service.GetByIdAsync(id));

        /// <summary>Registra un nuevo bloque.</summary>
        /// <response code="201">Registro creado.</response>
        /// <response code="400">Datos inválidos o regla de negocio incumplida.</response>
        [HttpPost]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SaveBloqueDto dto)
        {
            var result = await _service.SaveAsync(dto);
            if (!result.Success) return Respond(result);
            return CreatedAtAction(nameof(GetById), new { id = IdDe(result.Data) }, result);
        }

        /// <summary>Actualiza un bloque existente.</summary>
        /// <param name="id">Identificador del registro a actualizar.</param>
        /// <response code="200">Registro actualizado.</response>
        /// <response code="400">Datos inválidos o regla de negocio incumplida.</response>
        /// <response code="404">El registro no existe.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBloqueDto dto)
        {
            if (dto.Id != 0 && dto.Id != id)
                return BadRequestMessage("El id de la ruta no coincide con el id del cuerpo.");

            dto.Id = id;
            return Respond(await _service.UpdateAsync(dto));
        }

        /// <summary>Desactiva un bloque (borrado lógico).</summary>
        /// <param name="id">Identificador del registro a desactivar.</param>
        /// <response code="200">Registro desactivado.</response>
        /// <response code="400">No se puede desactivar por una regla de negocio.</response>
        /// <response code="404">El registro no existe.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remove(int id)
            => Respond(await _service.RemoveAsync(new RemoveDto { Id = id }));

        /// <summary>Reactiva un bloque previamente desactivado.</summary>
        /// <param name="id">Identificador del registro a activar.</param>
        /// <response code="200">Registro activado.</response>
        /// <response code="400">No se puede activar por una regla de negocio.</response>
        /// <response code="404">El registro no existe.</response>
        [HttpPatch("{id:int}/activar")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(int id)
            => Respond(await _service.ActivateAsync(new RemoveDto { Id = id }));
    }
}
