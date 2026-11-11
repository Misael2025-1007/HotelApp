using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories.Base;

namespace HotelApp.DAL.Repositories
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> GetAllConDetalleAsync();
        Task<bool> ExisteNumeroEnHotelAsync(int hotelId, string numero, int? excluirId = null);
    }
}