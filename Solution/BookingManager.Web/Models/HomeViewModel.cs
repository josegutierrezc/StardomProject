using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cars.DTO;

namespace BookingManager.Web.Models
{
    public class HomeViewModel
    {
        public int TotalUnconfirmedCarReservations { get; set; }
        public List<UnconfirmedCarReservation> CarReservationsPendingForConfirmation { get; set; }
        public bool CanSeePaymentsChart { get; set; }
    }

    public class UnconfirmedCarReservation {
        public long CarReservationId { get; set; }
        public string ClientFullname { get; set; }
        public string ClientPhone { get; set; }
        public string CarCategoryName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Days { get; set; }
    }
}