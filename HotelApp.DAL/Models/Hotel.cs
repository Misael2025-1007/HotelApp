using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models
{
    public class Hotel : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; }

        public ICollection<Bloque> Bloques { get; set; } = new List<Bloque>();
        public ICollection<Piso> Pisos { get; set; } = new List<Piso>();
    }
}