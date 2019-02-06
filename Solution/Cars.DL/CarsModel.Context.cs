﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CarsModel : DbContext
    {
        public CarsModel()
            : base("name=CarsModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CarCategory> CarCategories { get; set; }
        public virtual DbSet<CarCategoryReservationDay> CarCategoryReservationDays { get; set; }
        public virtual DbSet<CarProvider> CarProviders { get; set; }
        public virtual DbSet<CarProvidersCarCategory> CarProvidersCarCategories { get; set; }
        public virtual DbSet<CarReservation> CarReservations { get; set; }
        public virtual DbSet<CarTransmission> CarTransmissions { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ConfirmationStatus> ConfirmationStatuses { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<PaymentConcept> PaymentConcepts { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public virtual DbSet<PriceConfiguration> PriceConfigurations { get; set; }
        public virtual DbSet<Privilege> Privileges { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<RentCarPlace> RentCarPlaces { get; set; }
        public virtual DbSet<RentMEConfiguration> RentMEConfigurations { get; set; }
        public virtual DbSet<ReservationDay> ReservationDays { get; set; }
        public virtual DbSet<SeasonDate> SeasonDates { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<TourOperatorCarProvider> TourOperatorCarProviders { get; set; }
        public virtual DbSet<TourOperator> TourOperators { get; set; }
        public virtual DbSet<TourOperatorsParent> TourOperatorsParents { get; set; }
        public virtual DbSet<UserPrivilege> UserPrivileges { get; set; }
        public virtual DbSet<CarReservations_Log> CarReservations_Log { get; set; }
        public virtual DbSet<LinkedAgency> LinkedAgencies { get; set; }
    
        public virtual int GetPriceConfigurationForReservation(Nullable<int> carCategoryId, Nullable<int> tourOperatorId, Nullable<System.DateTime> from, Nullable<System.DateTime> to)
        {
            var carCategoryIdParameter = carCategoryId.HasValue ?
                new ObjectParameter("CarCategoryId", carCategoryId) :
                new ObjectParameter("CarCategoryId", typeof(int));
    
            var tourOperatorIdParameter = tourOperatorId.HasValue ?
                new ObjectParameter("TourOperatorId", tourOperatorId) :
                new ObjectParameter("TourOperatorId", typeof(int));
    
            var fromParameter = from.HasValue ?
                new ObjectParameter("From", from) :
                new ObjectParameter("From", typeof(System.DateTime));
    
            var toParameter = to.HasValue ?
                new ObjectParameter("To", to) :
                new ObjectParameter("To", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetPriceConfigurationForReservation", carCategoryIdParameter, tourOperatorIdParameter, fromParameter, toParameter);
        }
    }
}
