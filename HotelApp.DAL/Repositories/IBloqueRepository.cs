using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories.Base;

namespace HotelApp.DAL.Repositories
{
    public interface IBloqueRepository : IBaseRepository<Bloque>
    {
        Task<List<Bloque>> GetAllConHotelAsync();
        Task<bool> ExisteNombreEnHotelAsync(int hotelId, string nombre, int? excluirId = null);
    }
}