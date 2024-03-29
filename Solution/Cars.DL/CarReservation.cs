//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cars.DL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CarReservation
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.DateTime> CancelledOn { get; set; }
        public Nullable<int> CancelledBy { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public int ClientId { get; set; }
        public string ReservationNumber { get; set; }
        public string FlightNumber { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public Nullable<int> CarProviderId { get; set; }
        public Nullable<int> CarCategoryId { get; set; }
        public Nullable<int> TourOperatorId { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public Nullable<int> RentCarPlaceId { get; set; }
        public Nullable<bool> HasInsurance { get; set; }
        public Nullable<double> CostPrice { get; set; }
        public Nullable<double> SalePrice { get; set; }
        public Nullable<double> InsuranceCost { get; set; }
        public Nullable<double> Discount { get; set; }
        public string Note { get; set; }
    }
}
