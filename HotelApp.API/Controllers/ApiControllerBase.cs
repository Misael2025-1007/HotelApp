using HotelApp.BL.Base;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    /// <summary>
    /// Traduce el OperationResult de la capa de negocio a un código HTTP.
    /// Éxito = 200, recurso inexistente = 404, regla de negocio incumplida = 400, error inesperado = 500.
    /// </summary>
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult Respond(OperationResult result)
        {
            if (result.Success) return Ok(result);
            return StatusCode(CodigoDeError(result.Message), result);
        }

        protected IActionResult BadRequestMessage(string message)
            => BadRequest(new OperationResult { Success = false, Message = message });

        protected static int? IdDe(object? data)
            => data?.GetType().GetProperty("Id")?.GetValue(data) as int?;

        private static int CodigoDeError(string? message)
        {
            if (string.IsNullOrWhiteSpace(message)) return StatusCodes.Status400BadRequest;

            if (message.StartsWith("Ocurrió un error", StringComparison.OrdinalIgnoreCase))
                return StatusCodes.Status500InternalServerError;

            // "El piso no existe." => 404. "El hotel seleccionado no existe." => dato inválido del cuerpo (400)
            if (message.EndsWith("no existe.", StringComparison.OrdinalIgnoreCase) &&
                !message.Contains("seleccionad", StringComparison.OrdinalIgnoreCase))
                return StatusCodes.Status404NotFound;

            return StatusCodes.Status400BadRequest;
        }
    }
}
