using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models
{
    public class Piso : AuditEntity
    {
        public int HotelId { get; set; }
        public int? BloqueId { get; set; }
        public int Numero { get; set; }

        public Hotel Hotel { get; set; } = null!;
        public Bloque? Bloque { get; set; }
        public ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();
    }
}