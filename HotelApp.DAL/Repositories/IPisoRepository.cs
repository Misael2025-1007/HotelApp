using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories.Base;

namespace HotelApp.DAL.Repositories
{
    public interface IPisoRepository : IBaseRepository<Piso>
    {
        Task<List<Piso>> GetAllConDetalleAsync();
        Task<bool> ExisteNumeroActivoAsync(int hotelId, int numero, int? excluirId = null);
    }
}
