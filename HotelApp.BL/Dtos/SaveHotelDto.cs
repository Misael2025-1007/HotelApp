using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace HotelApp.BL.Dtos
{
    public class SaveHotelDto : DtoBase
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede pasar de 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(250, ErrorMessage = "La dirección no puede pasar de 250 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El teléfono no puede pasar de 20 caracteres.")]
        public string? Telefono { get; set; }
    }
}