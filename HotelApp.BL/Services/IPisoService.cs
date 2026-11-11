using HotelApp.BL.Base;
using HotelApp.BL.Dtos;

namespace HotelApp.BL.Services
{
    public interface IPisoService
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> SaveAsync(SavePisoDto dto);
        Task<OperationResult> UpdateAsync(UpdatePisoDto dto);
        Task<OperationResult> RemoveAsync(RemoveDto dto);
        Task<OperationResult> ActivateAsync(RemoveDto dto);
    }
}
