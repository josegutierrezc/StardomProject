using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookingManager.Web.Models
{
    public class PriceConfigurationViewModel
    {
        [Display(Name = "Tour Operador:")]
        public int TourOperatorId { get; set; }

        [Display(Name = "Temporada:")]
        public int SeasonId { get; set; }

        public Dictionary<int, KeyValuePair<int, PriceColumnModel>> Columns { get; set; }
        public Dictionary<int, KeyValuePair<int, PriceRowModel>> Rows { get; set; }
        public List<List<PriceDataModel>> Data { get; set; }
    }
}