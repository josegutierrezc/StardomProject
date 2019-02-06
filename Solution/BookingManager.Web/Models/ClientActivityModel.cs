using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingManager.Web.Models
{
    public class ClientActivityModel
    {
        public bool IsCarReservationActivity { get; set; }
        public string CarReservationCreatedByUser { get; set; }
        public string CarReservationCreatedOn { get; set; }
        public bool Cancelled { get; set; }
        public string CarReservationNumber { get; set; }
        public string CarReservationFromDate { get; set; }
        public string CarReservationToDate { get; set; }
        public string CarReservationCarCategory { get; set; }
        public string CarReservationRentCarPlace { get; set; }
        public double CarReservationSalePrice { get; set; }
        public double CarReservationDiscount { get; set; }
        public string CarReservationNote { get; set; }
        public int CarReservationTotalDays { get; set; }
    }
}