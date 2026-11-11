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
    public class TipoHabitacionRepository : BaseRepository<TipoHabitacion>, ITipoHabitacionRepository
    {
        public TipoHabitacionRepository(HotelDbContext context) : base(context) { }

        public Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null)
            => ExistsAsync(t => t.Nombre == nombre && t.Status && t.Id != excluirId);
    }
}