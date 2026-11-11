using System.ComponentModel.DataAnnotations;
using HotelApp.DAL.Models.Enums;

namespace HotelApp.BL.Dtos
{
    public class HabitacionDto
    {
        public int Id { get; set; }
        public int PisoId { get; set; }
        public int PisoNumero { get; set; }
        public int HotelId { get; set; }
        public string HotelNombre { get; set; } = string.Empty;
        public int TipoHabitacionId { get; set; }
        public string TipoNombre { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public decimal Tarifa { get; set; }
        public EstadoHabitacion Estado { get; set; }
        public bool Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }

    public class SaveHabitacionDto : DtoBase
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un piso.")]
        public int PisoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de habitación.")]
        public int TipoHabitacionId { get; set; }

        [Required(ErrorMessage = "El número de habitación es obligatorio.")]
        [StringLength(10, ErrorMessage = "El número no puede pasar de 10 caracteres.")]
        public string Numero { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "La capacidad debe estar entre 1 y 20.")]
        public int Capacidad { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "La tarifa debe ser mayor que cero.")]
        public decimal Tarifa { get; set; }

        public EstadoHabitacion Estado { get; set; } = EstadoHabitacion.Available;
    }

    public class UpdateHabitacionDto : SaveHabitacionDto
    {
        public int Id { get; set; }
    }
}
