using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DTO
{
    public class ReservationDayDTO
    {
        public int Id { get; set; }
        public int FromDay { get; set; }
        public int ToDay { get; set; }
        public bool IsActive { get; set; }
    }
}
