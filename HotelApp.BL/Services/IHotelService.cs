using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.BL.Base;
using HotelApp.BL.Dtos;

namespace HotelApp.BL.Services
{
    public interface IHotelService
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> SaveAsync(SaveHotelDto dto);
        Task<OperationResult> UpdateAsync(UpdateHotelDto dto);
        Task<OperationResult> RemoveAsync(RemoveDto dto);    // desactivar
        Task<OperationResult> ActivateAsync(RemoveDto dto);  // reactivar
    }
}