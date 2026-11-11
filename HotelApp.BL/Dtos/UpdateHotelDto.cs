using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.BL.Dtos
{
    public class UpdateHotelDto : SaveHotelDto
    {
        public int Id { get; set; }
    }
}