using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace HotelApp.BL.Dtos
{
    public class SaveBloqueDto : DtoBase
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un hotel.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede pasar de 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "La descripción no puede pasar de 250 caracteres.")]
        public string? Descripcion { get; set; }
    }
}