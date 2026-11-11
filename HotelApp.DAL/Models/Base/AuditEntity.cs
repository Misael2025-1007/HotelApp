using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.DAL.Models.Base
{
    public abstract class AuditEntity
    {
        public int Id { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? ModifyDate { get; set; }
        public int? UserMod { get; set; }
    }
}