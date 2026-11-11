using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models
{
    public class Bloque : AuditEntity
    {
        public int HotelId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public Hotel Hotel { get; set; } = null!;
        public ICollection<Piso> Pisos { get; set; } = new List<Piso>();
    }
}