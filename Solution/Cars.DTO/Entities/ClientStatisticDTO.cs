using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DTO
{
    public class ClientStatisticDTO
    {
        public int ClientId { get; set; }
        public DateTime ClientSince { get; set; }
        public int TotalCarReservations { get; set; }
        public int TotalCarReservationDays { get; set; }
        public int AverageCarReservationDays { get; set; }
        public int AverageCarReservationsPerYear { get; set; }
        public string PreferedCarCategory { get; set; }
    }
}
