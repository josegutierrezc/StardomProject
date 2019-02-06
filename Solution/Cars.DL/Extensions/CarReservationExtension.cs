using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class CarReservationExtension
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string ModifiedByUser { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.DateTime> CancelledOn { get; set; }
        public Nullable<int> CancelledBy { get; set; }
        public string CancelledByUser { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public int ClientId { get; set; }
        public string ClientFirstname { get; set; }
        public string ClientLastname { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public bool TroubledClient { get; set; }
        public string ReservationNumber { get; set; }
        public string FlightNumber { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int Days { get; set; }
        public Nullable<int> CarProviderId { get; set; }
        public string CarProviderName { get; set; }
        public Nullable<int> CarCategoryId { get; set; }
        public string CarCategoryName { get; set; }
        public Nullable<int> TourOperatorId { get; set; }
        public string TourOperatorName { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public Nullable<int> RentCarPlaceId { get; set; }
        public string RentCarPlaceName { get; set; }
        public Nullable<bool> HasInsurance { get; set; }
        public Nullable<double> CostPrice { get; set; }
        public Nullable<double> SalePrice { get; set; }
        public Nullable<double> InsuranceCost { get; set; }
        public Nullable<double> Discount { get; set; }
        public string Note { get; set; }
        public string LinkedAgencyName { get; set; }
        public string LinkedAgencyPhone { get; set; }
        public string LinkedAgencyEmail { get; set; }
    }
}
