using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DTO
{
    public partial class PriceConfigurationDTO
    {
        public int Id { get; set; }
        public int CarCategoryId { get; set; }
        public int TourOperatorId { get; set; }
        public int SeasonId { get; set; }
        public int ReservationDayId { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
