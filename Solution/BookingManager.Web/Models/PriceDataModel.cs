using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingManager.Web.Models
{
    public class PriceColumnModel
    {
        public int ReservationDayId { get; set; }
        public string Description { get; set; }
    }

    public class PriceRowModel
    {
        public int CarCategoryId { get; set; }
        public string Description { get; set; }
    }

    public class PriceDataModel
    {
        public int ReservationDayId { get; set; }
        public int CarCategoryId { get; set; }
        public double? CostPrice { get; set; }
        public double? SalePrice { get; set; }
    }
}