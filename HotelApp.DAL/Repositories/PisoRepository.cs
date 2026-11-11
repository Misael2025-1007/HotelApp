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
    public class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        public PisoRepository(HotelDbContext context) : base(context) { }

        public async Task<List<Piso>> GetAllConDetalleAsync()
            => await _dbSet.AsNoTracking()
                .Include(p => p.Hotel)
                .Include(p => p.Bloque)
                .ToListAsync();

        // RN03: no pueden existir dos pisos activos con el mismo número en un hotel
        public Task<bool> ExisteNumeroActivoAsync(int hotelId, int numero, int? excluirId = null)
            => ExistsAsync(p => p.HotelId == hotelId && p.Numero == numero && p.Status && p.Id != excluirId);
    }
}