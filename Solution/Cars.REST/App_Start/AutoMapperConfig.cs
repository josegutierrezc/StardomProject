using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppsManager.DL;
using AppsManager.DTO;
using Cars.DL;
using Cars.DTO;
using AutoMapper;

namespace Cars.REST
{
    public static class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(cfg => {
                cfg.AddProfile(new AppsManagerProfile());
                cfg.AddProfile(new CarsProfile());
            });
        }
    }

    public class AppsManagerProfile : Profile
    {
        public AppsManagerProfile()
        {
            CreateMap<Agency, AgencyDTO>().ReverseMap();
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<UserAgencyHelper, UserAgencyDTO>();
        }
    }

    public class CarsProfile : Profile
    {
        public CarsProfile()
        {
            CreateMap<ClientExtension, ClientDTO>();
            CreateMap<Client, ClientDTO>();
            CreateMap<Privilege, PrivilegeDTO>();
            CreateMap<CarReservationExtension, CarReservationDTO>();
            CreateMap<CarReservationExtension, CarReservation>();
            CreateMap<CarCategory, CarCategoryDTO>();
            CreateMap<CarProvider, CarProviderDTO>();
            CreateMap<TourOperator, TourOperatorDTO>();
            CreateMap<Province, ProvinceDTO>();
            CreateMap<RentCarPlace, RentCarPlaceDTO>();
            CreateMap<PriceConfiguration, PriceConfigurationDTO>();
            CreateMap<PaymentExtension, CarReservationPaymentDTO>();
            CreateMap<Payment, CarReservationPaymentDTO>();
            CreateMap<PaymentConcept, PaymentConceptDTO>();
            CreateMap<PaymentMethod, PaymentMethodDTO>();
            CreateMap<LinkedAgency, LinkedAgencyDTO>();
            CreateMap<Season, SeasonDTO>();
            CreateMap<ReservationDay, ReservationDayDTO>();
            CreateMap<PaymentStatus, PaymentStatusDTO>();
            CreateMap<CarReservationsPerAgent, CarReservationsPerAgentDTO>();
            CreateMap<PaymentsPerPeriod, PaymentsPerPeriodDTO>();
        }
    }
}