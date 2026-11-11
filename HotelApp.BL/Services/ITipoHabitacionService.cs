using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.BL.Base;
using HotelApp.BL.Dtos;

namespace HotelApp.BL.Services
{
    public interface ITipoHabitacionService
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> SaveAsync(SaveTipoHabitacionDto dto);
        Task<OperationResult> UpdateAsync(UpdateTipoHabitacionDto dto);
        Task<OperationResult> RemoveAsync(RemoveDto dto);
        Task<OperationResult> ActivateAsync(RemoveDto dto);
    }
}