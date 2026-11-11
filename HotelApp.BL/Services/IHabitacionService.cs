using HotelApp.BL.Base;
using HotelApp.BL.Dtos;

namespace HotelApp.BL.Services
{
    public interface IHabitacionService
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> SaveAsync(SaveHabitacionDto dto);
        Task<OperationResult> UpdateAsync(UpdateHabitacionDto dto);
        Task<OperationResult> RemoveAsync(RemoveDto dto);
        Task<OperationResult> ActivateAsync(RemoveDto dto);
    }
}
