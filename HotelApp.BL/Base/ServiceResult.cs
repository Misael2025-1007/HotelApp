using HotelApp.BL.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HotelApp.BL.Base
{
    public static class ServiceResult
    {
        public static OperationResult Ok(string? message = null, object? data = null)
            => new OperationResult { Success = true, Message = message, Data = data };

        public static OperationResult Fail(string message)
            => new OperationResult { Success = false, Message = message };

        // Registra un rechazo de regla de negocio y devuelve el error
        public static OperationResult Reject(ILogger logger, string operacion, string motivo, int? id = null)
        {
            logger.LogWarning("Validación fallida en {Operacion} (Id: {Id}): {Motivo}", operacion, id, motivo);
            return Fail(motivo);
        }

        // Ejecuta la operación; si lanza una excepción la registra en el log y devuelve un error controlado
        public static async Task<OperationResult> TryAsync(
            Func<Task<OperationResult>> action, ILogger logger, string errorMessage)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Fail(errorMessage);
            }
        }
    }
}