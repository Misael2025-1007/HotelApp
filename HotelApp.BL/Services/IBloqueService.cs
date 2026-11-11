using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.BL.Base;
using HotelApp.BL.Dtos;

namespace HotelApp.BL.Services
{
    public interface IBloqueService
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> SaveAsync(SaveBloqueDto dto);
        Task<OperationResult> UpdateAsync(UpdateBloqueDto dto);
        Task<OperationResult> RemoveAsync(RemoveDto dto);
        Task<OperationResult> ActivateAsync(RemoveDto dto);
    }
}