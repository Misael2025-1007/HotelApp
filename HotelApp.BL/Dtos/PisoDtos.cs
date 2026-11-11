using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace HotelApp.BL.Dtos
{
    public class PisoDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelNombre { get; set; } = string.Empty;
        public int? BloqueId { get; set; }
        public string? BloqueNombre { get; set; }
        public int Numero { get; set; }
        public bool Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }

    public class SavePisoDto : DtoBase
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un hotel.")]
        public int HotelId { get; set; }

        public int? BloqueId { get; set; }

        [Range(0, 200, ErrorMessage = "El número de piso debe estar entre 0 y 200.")]
        public int Numero { get; set; }
    }

    public class UpdatePisoDto : SavePisoDto
    {
        public int Id { get; set; }
    }
}