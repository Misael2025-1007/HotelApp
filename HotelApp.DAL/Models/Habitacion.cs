using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Enums;

namespace HotelApp.DAL.Models
{
    public class Habitacion : AuditEntity
    {
        public int PisoId { get; set; }
        public int TipoHabitacionId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public decimal Tarifa { get; set; }
        public EstadoHabitacion Estado { get; set; } = EstadoHabitacion.Available;

        public Piso Piso { get; set; } = null!;
        public TipoHabitacion TipoHabitacion { get; set; } = null!;
    }
}
