using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Data;
using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories.Base;

namespace HotelApp.DAL.Repositories
{
    public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(HotelDbContext context) : base(context) { }

        public Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null)
            => ExistsAsync(h => h.Nombre == nombre && h.Status && h.Id != excluirId);
    }
}