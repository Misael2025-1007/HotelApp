using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Data;
using HotelApp.DAL.Models;
using HotelApp.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.DAL.Repositories
{
    public class BloqueRepository : BaseRepository<Bloque>, IBloqueRepository
    {
        public BloqueRepository(HotelDbContext context) : base(context) { }

        public async Task<List<Bloque>> GetAllConHotelAsync()
            => await _dbSet.AsNoTracking().Include(b => b.Hotel).ToListAsync();

        public Task<bool> ExisteNombreEnHotelAsync(int hotelId, string nombre, int? excluirId = null)
            => ExistsAsync(b => b.HotelId == hotelId && b.Nombre == nombre && b.Status && b.Id != excluirId);
    }
}