using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cars.DTO;

namespace BookingManager.Web.Models
{
    public class CarReservationsLogModel
    {
        public List<CarReservationDTO> ActiveReservations { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string SearchFor { get; set; }
        public string Filter { get; set; }

        public List<CarReservationDTO> HistoryReservations { get; set; }
        public int HistoryPageSize { get; set; }
        public int HistoryPageNumber { get; set; }
        public int HistoryTotalPages { get; set; }
        public string HistorySearchFor { get; set; }
        public string HistoryFilter { get; set; }
    }
}