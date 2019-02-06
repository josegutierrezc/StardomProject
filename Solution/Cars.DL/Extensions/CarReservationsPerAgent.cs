using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class CarReservationsPerAgent
    {
        public int FirstYear { get; set; }
        public int SecondYear { get; set; }
        public int Month { get; set; }
        public int[] FirstYearReservations { get; set; }
        public int[] SecondYearReservations { get; set; }
    }
}
