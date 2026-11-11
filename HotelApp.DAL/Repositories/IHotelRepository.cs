using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories.Base;

namespace HotelApp.DAL.Repositories
{
    public interface IHotelRepository : IBaseRepository<Hotel>
    {
        Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null);
    }
}