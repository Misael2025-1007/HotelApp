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
    public class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        public HabitacionRepository(HotelDbContext context) : base(context) { }

        public async Task<List<Habitacion>> GetAllConDetalleAsync()
            => await _dbSet.AsNoTracking()
                .Include(h => h.Piso).ThenInclude(p => p.Hotel)
                .Include(h => h.TipoHabitacion)
                .ToListAsync();

        // RN05: el número de habitación debe ser único dentro del hotel
        public Task<bool> ExisteNumeroEnHotelAsync(int hotelId, string numero, int? excluirId = null)
            => ExistsAsync(h => h.Piso.HotelId == hotelId && h.Numero == numero && h.Status && h.Id != excluirId);
    }
}