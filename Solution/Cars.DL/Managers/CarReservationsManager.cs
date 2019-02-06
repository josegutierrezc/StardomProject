using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class CarReservationsManager
    {
        public const string TypeAll = "ALL";
        public const string TypeActive = "ACTIVE";
        public const string TypeHistory = "HISTORY";
        public const string FilterNone = "";
        public const string FilterConfirmed = "CONFIRMED";
        public const string FilterUnconfirmed = "UNCONFIRMED";
        public const string FilterFullyPaid = "FULLYPAID";
        public const string FilterPartiallyPaid = "PARTIALLYPAID";
        public const string FilterUnpaid = "UNPAID";
        public const string FilterCancelled = "CANCELLED";
        public const string FilterPendingRefund = "PENDINGREFUND";
        public const int PaymentStatusAll = -1;
        public const int PaymentStatusUnpaid = 1;
        public const int PaymentStatusPartiallyPaid = 2;
        public const int PaymentStatusPaid = 3;
        public const int PaymentStatusPendingRefund = 4;
        public const string PaymentStatusAllName = "Todos";
        public const string PaymentStatusUnpaidName = "No pagada";
        public const string PaymentStatusPartiallyPaidName = "Parcialmente pagada";
        public const string PaymentStatusPaidName = "Pagada";
        public const string PaymentStatusPendingRefundName = "Pendiente a reembolso";
        public static string[] PaymentStatusNames = new string[] { PaymentStatusAllName, PaymentStatusUnpaidName, PaymentStatusPartiallyPaidName, PaymentStatusPaidName, PaymentStatusPendingRefundName };
        public KeyValuePair<int, List<CarReservationExtension>> Get(string AgencyNumber, string Type, string SearchFor, int PageSize, int PageNumber, string Filter) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                IQueryable<CarReservationExtension> query = null;
                if (Type.ToUpper() == TypeActive)
                {
                    query = from cr in DB.CarReservations
                            join c in DB.Clients on cr.ClientId equals c.Id
                            join cp in DB.CarProviders on cr.CarProviderId equals cp.Id into cproviders
                            from subpro in cproviders.DefaultIfEmpty()
                            join cc in DB.CarCategories on cr.CarCategoryId equals cc.Id into ccat
                            from subcat in ccat.DefaultIfEmpty()
                            join to in DB.TourOperators on cr.TourOperatorId equals to.Id into tope
                            from subtour in tope.DefaultIfEmpty()
                            join p in DB.Provinces on cr.ProvinceId equals p.Id into prov
                            from subprov in prov.DefaultIfEmpty()
                            join rcp in DB.RentCarPlaces on cr.RentCarPlaceId equals rcp.Id into ren
                            from subrent in ren.DefaultIfEmpty()
                            where cr.ToDate >= DateTime.Now
                            select new CarReservationExtension
                            {
                                Id = cr.Id,
                                CreatedBy = cr.CreatedBy,
                                CreatedByUser = string.Empty,
                                CreatedOn = cr.CreatedOn,
                                ModifiedBy = cr.ModifiedBy,
                                ModifiedByUser = string.Empty,
                                ModifiedOn = cr.ModifiedOn,
                                CancelledOn = cr.CancelledOn,
                                CancelledBy = cr.CancelledBy,
                                CancelledByUser = string.Empty,
                                PaymentStatusId = cr.PaymentStatusId,
                                ClientId = cr.ClientId,
                                ClientFirstname = c.FirstName,
                                ClientLastname = c.LastName,
                                ClientPhone = c.Phone,
                                ClientEmail = c.Email,
                                TroubledClient = c.Troubled,
                                ReservationNumber = cr.ReservationNumber,
                                FlightNumber = cr.FlightNumber,
                                FromDate = cr.FromDate,
                                ToDate = cr.ToDate,
                                CarProviderId = cr.CarProviderId,
                                CarProviderName = subpro.Name,
                                CarCategoryId = cr.CarCategoryId,
                                CarCategoryName = subcat.Name,
                                TourOperatorId = cr.TourOperatorId,
                                TourOperatorName = subtour.Name,
                                ProvinceId = cr.ProvinceId,
                                ProvinceName = subprov.Name,
                                RentCarPlaceId = cr.RentCarPlaceId,
                                RentCarPlaceName = subrent.Name,
                                HasInsurance = cr.HasInsurance,
                                CostPrice = cr.CostPrice,
                                SalePrice = cr.SalePrice,
                                InsuranceCost = cr.InsuranceCost,
                                Discount = cr.Discount,
                                Note = cr.Note
                            };
                }
                else if (Type.ToUpper() == TypeHistory) {
                    query = from cr in DB.CarReservations
                            join c in DB.Clients on cr.ClientId equals c.Id
                            join cp in DB.CarProviders on cr.CarProviderId equals cp.Id into cproviders
                            from subpro in cproviders.DefaultIfEmpty()
                            join cc in DB.CarCategories on cr.CarCategoryId equals cc.Id into ccat
                            from subcat in ccat.DefaultIfEmpty()
                            join to in DB.TourOperators on cr.TourOperatorId equals to.Id into tope
                            from subtour in tope.DefaultIfEmpty()
                            join p in DB.Provinces on cr.ProvinceId equals p.Id into prov
                            from subprov in prov.DefaultIfEmpty()
                            join rcp in DB.RentCarPlaces on cr.RentCarPlaceId equals rcp.Id into ren
                            from subrent in ren.DefaultIfEmpty()
                            where cr.ToDate < DateTime.Today
                            select new CarReservationExtension
                            {
                                Id = cr.Id,
                                CreatedBy = cr.CreatedBy,
                                CreatedByUser = string.Empty,
                                CreatedOn = cr.CreatedOn,
                                ModifiedBy = cr.ModifiedBy,
                                ModifiedByUser = string.Empty,
                                ModifiedOn = cr.ModifiedOn,
                                CancelledOn = cr.CancelledOn,
                                CancelledBy = cr.CancelledBy,
                                CancelledByUser = string.Empty,
                                PaymentStatusId = cr.PaymentStatusId,
                                ClientId = cr.ClientId,
                                ClientFirstname = c.FirstName,
                                ClientLastname = c.LastName,
                                ClientPhone = c.Phone,
                                ClientEmail = c.Email,
                                TroubledClient = c.Troubled,
                                ReservationNumber = cr.ReservationNumber,
                                FlightNumber = cr.FlightNumber,
                                FromDate = cr.FromDate,
                                ToDate = cr.ToDate,
                                CarProviderId = cr.CarProviderId,
                                CarProviderName = subpro.Name,
                                CarCategoryId = cr.CarCategoryId,
                                CarCategoryName = subcat.Name,
                                TourOperatorId = cr.TourOperatorId,
                                TourOperatorName = subtour.Name,
                                ProvinceId = cr.ProvinceId,
                                ProvinceName = subprov.Name,
                                RentCarPlaceId = cr.RentCarPlaceId,
                                RentCarPlaceName = subrent.Name,
                                HasInsurance = cr.HasInsurance,
                                CostPrice = cr.CostPrice,
                                SalePrice = cr.SalePrice,
                                InsuranceCost = cr.InsuranceCost,
                                Discount = cr.Discount,
                                Note = cr.Note
                            };
                }

                if (SearchFor != null && SearchFor != string.Empty) query = query.Where(e => e.ClientFirstname.Contains(SearchFor) | e.ClientLastname.Contains(SearchFor) | (e.ClientFirstname + " " + e.ClientLastname).Contains(SearchFor) | e.ClientPhone.Contains(SearchFor) | e.ReservationNumber.Contains(SearchFor));
                if (Filter != null && Filter != string.Empty)
                {
                    if (Filter.ToUpper() == FilterConfirmed) query = query.Where(e => e.ReservationNumber != null && e.ReservationNumber.Trim() != string.Empty && e.CancelledOn == null);
                    else if (Filter.ToUpper() == FilterUnconfirmed) query = query.Where(e => (e.ReservationNumber == null || e.ReservationNumber.Trim() == string.Empty) & (e.CancelledOn == null));
                    else if (Filter.ToUpper() == FilterFullyPaid) query = query.Where(e => e.PaymentStatusId == 3);
                    else if (Filter.ToUpper() == FilterPartiallyPaid) query = query.Where(e => e.PaymentStatusId == 2);
                    else if (Filter.ToUpper() == FilterUnpaid) query = query.Where(e => e.PaymentStatusId == 1);
                    else if (Filter.ToUpper() == FilterCancelled) query = query.Where(e => e.CancelledOn != null);
                    else if (Filter.ToUpper() == FilterPendingRefund) query = query.Where(e => e.PaymentStatusId == 4);
                }
                int totalRecords = query.Count();
                return new KeyValuePair<int, List<CarReservationExtension>>(totalRecords, query.OrderByDescending(e => e.CreatedOn).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList());
            }
        }
        public KeyValuePair<int, List<CarReservationExtension>> GetAccountingReport(string AgencyNumber, DateTime FromDate, DateTime ToDate, int TourOperatorId, int PaymentStatusId)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                var query = from cr in DB.CarReservations
                        join c in DB.Clients on cr.ClientId equals c.Id
                        join cp in DB.CarProviders on cr.CarProviderId equals cp.Id into cproviders
                        from subpro in cproviders.DefaultIfEmpty()
                        join cc in DB.CarCategories on cr.CarCategoryId equals cc.Id into ccat
                        from subcat in ccat.DefaultIfEmpty()
                        join to in DB.TourOperators on cr.TourOperatorId equals to.Id into tope
                        from subtour in tope.DefaultIfEmpty()
                        join p in DB.Provinces on cr.ProvinceId equals p.Id into prov
                        from subprov in prov.DefaultIfEmpty()
                        join rcp in DB.RentCarPlaces on cr.RentCarPlaceId equals rcp.Id into ren
                        from subrent in ren.DefaultIfEmpty()
                        where cr.FromDate >= FromDate & cr.FromDate <= ToDate & cr.CancelledOn == null
                        select new CarReservationExtension
                        {
                            Id = cr.Id,
                            CreatedBy = cr.CreatedBy,
                            CreatedByUser = string.Empty,
                            CreatedOn = cr.CreatedOn,
                            ModifiedBy = cr.ModifiedBy,
                            ModifiedByUser = string.Empty,
                            ModifiedOn = cr.ModifiedOn,
                            CancelledOn = cr.CancelledOn,
                            CancelledBy = cr.CancelledBy,
                            CancelledByUser = string.Empty,
                            PaymentStatusId = cr.PaymentStatusId,
                            ClientId = cr.ClientId,
                            ClientFirstname = c.FirstName,
                            ClientLastname = c.LastName,
                            ClientPhone = c.Phone,
                            ClientEmail = c.Email,
                            TroubledClient = c.Troubled,
                            ReservationNumber = cr.ReservationNumber,
                            FlightNumber = cr.FlightNumber,
                            FromDate = cr.FromDate,
                            ToDate = cr.ToDate,
                            CarProviderId = cr.CarProviderId,
                            CarProviderName = subpro.Name,
                            CarCategoryId = cr.CarCategoryId,
                            CarCategoryName = subcat.Name,
                            TourOperatorId = cr.TourOperatorId,
                            TourOperatorName = subtour.Name,
                            ProvinceId = cr.ProvinceId,
                            ProvinceName = subprov.Name,
                            RentCarPlaceId = cr.RentCarPlaceId,
                            RentCarPlaceName = subrent.Name,
                            HasInsurance = cr.HasInsurance,
                            CostPrice = cr.CostPrice,
                            SalePrice = cr.SalePrice,
                            InsuranceCost = cr.InsuranceCost,
                            Discount = cr.Discount,
                            Note = cr.Note
                        };
                if (TourOperatorId != -1) query = query.Where(e => e.TourOperatorId == TourOperatorId);
                if (PaymentStatusId != PaymentStatusAll) query = query.Where(e => e.PaymentStatusId == PaymentStatusId);
                int totalRecords = query.Count();
                return new KeyValuePair<int, List<CarReservationExtension>>(totalRecords, query.OrderBy(e => e.FromDate).ToList());
            }
        }
        public CarReservationExtension GetById(string AgencyNumber, long Id)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                var query = from cr in DB.CarReservations
                            join c in DB.Clients on cr.ClientId equals c.Id
                            join cp in DB.CarProviders on cr.CarProviderId equals cp.Id into cproviders from subpro in cproviders.DefaultIfEmpty()
                            join cc in DB.CarCategories on cr.CarCategoryId equals cc.Id into ccat from subcat in ccat.DefaultIfEmpty()
                            join to in DB.TourOperators on cr.TourOperatorId equals to.Id into tope from subtour in tope.DefaultIfEmpty()
                            join p in DB.Provinces on cr.ProvinceId equals p.Id into prov from subprov in prov.DefaultIfEmpty()
                            join rcp in DB.RentCarPlaces on cr.RentCarPlaceId equals rcp.Id into ren from subrent in ren.DefaultIfEmpty()
                            join la in DB.LinkedAgencies on c.LinkedToAgencyId equals la.Id into linka from linkeda in linka.DefaultIfEmpty()
                            where cr.Id == Id
                            select new CarReservationExtension
                            {
                                Id = cr.Id,
                                CreatedBy = cr.CreatedBy,
                                CreatedByUser = string.Empty,
                                CreatedOn = cr.CreatedOn,
                                ModifiedBy = cr.ModifiedBy,
                                ModifiedByUser = string.Empty,
                                ModifiedOn = cr.ModifiedOn,
                                CancelledOn = cr.CancelledOn,
                                CancelledBy = cr.CancelledBy,
                                CancelledByUser = string.Empty,
                                PaymentStatusId = cr.PaymentStatusId,
                                ClientId = cr.ClientId,
                                ClientFirstname = c.FirstName,
                                ClientLastname = c.LastName,
                                ClientPhone = c.Phone,
                                ClientEmail = c.Email,
                                TroubledClient = c.Troubled,
                                ReservationNumber = cr.ReservationNumber,
                                FlightNumber = cr.FlightNumber,
                                FromDate = cr.FromDate,
                                ToDate = cr.ToDate,
                                CarProviderId = cr.CarProviderId,
                                CarProviderName = subpro.Name,
                                CarCategoryId = cr.CarCategoryId,
                                CarCategoryName = subcat.Name,
                                TourOperatorId = cr.TourOperatorId,
                                TourOperatorName = subtour.Name,
                                ProvinceId = cr.ProvinceId,
                                ProvinceName = subprov.Name,
                                RentCarPlaceId = cr.RentCarPlaceId,
                                RentCarPlaceName = subrent.Name,
                                HasInsurance = cr.HasInsurance,
                                CostPrice = cr.CostPrice,
                                SalePrice = cr.SalePrice,
                                InsuranceCost = cr.InsuranceCost,
                                Discount = cr.Discount,
                                Note = cr.Note,
                                LinkedAgencyName = linkeda.Name,
                                LinkedAgencyPhone = linkeda.Phone,
                                LinkedAgencyEmail = linkeda.Email
                            };
                return query.FirstOrDefault();
            }
        }
        public List<CarReservationExtension> GetByClientId(string AgencyNumber, int ClientId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                var query = from cr in DB.CarReservations
                            join c in DB.Clients on cr.ClientId equals c.Id
                            join cp in DB.CarProviders on cr.CarProviderId equals cp.Id into cproviders
                            from subpro in cproviders.DefaultIfEmpty()
                            join cc in DB.CarCategories on cr.CarCategoryId equals cc.Id into ccat
                            from subcat in ccat.DefaultIfEmpty()
                            join to in DB.TourOperators on cr.TourOperatorId equals to.Id into tope
                            from subtour in tope.DefaultIfEmpty()
                            join p in DB.Provinces on cr.ProvinceId equals p.Id into prov
                            from subprov in prov.DefaultIfEmpty()
                            join rcp in DB.RentCarPlaces on cr.RentCarPlaceId equals rcp.Id into ren
                            from subrent in ren.DefaultIfEmpty()
                            where cr.ClientId == ClientId
                            select new CarReservationExtension
                            {
                                Id = cr.Id,
                                CreatedBy = cr.CreatedBy,
                                CreatedByUser = string.Empty,
                                CreatedOn = cr.CreatedOn,
                                ModifiedBy = cr.ModifiedBy,
                                ModifiedByUser = string.Empty,
                                ModifiedOn = cr.ModifiedOn,
                                CancelledOn = cr.CancelledOn,
                                CancelledBy = cr.CancelledBy,
                                CancelledByUser = string.Empty,
                                PaymentStatusId = cr.PaymentStatusId,
                                ClientId = cr.ClientId,
                                ClientFirstname = c.FirstName,
                                ClientLastname = c.LastName,
                                ClientPhone = c.Phone,
                                ClientEmail = c.Email,
                                TroubledClient = c.Troubled,
                                ReservationNumber = cr.ReservationNumber,
                                FlightNumber = cr.FlightNumber,
                                FromDate = cr.FromDate,
                                ToDate = cr.ToDate,
                                CarProviderId = cr.CarProviderId,
                                CarProviderName = subpro.Name,
                                CarCategoryId = cr.CarCategoryId,
                                CarCategoryName = subcat.Name,
                                TourOperatorId = cr.TourOperatorId,
                                TourOperatorName = subtour.Name,
                                ProvinceId = cr.ProvinceId,
                                ProvinceName = subprov.Name,
                                RentCarPlaceId = cr.RentCarPlaceId,
                                RentCarPlaceName = subrent.Name,
                                HasInsurance = cr.HasInsurance,
                                CostPrice = cr.CostPrice,
                                SalePrice = cr.SalePrice,
                                InsuranceCost = cr.InsuranceCost,
                                Discount = cr.Discount,
                                Note = cr.Note
                            };
                return query.ToList();
            }
        }
        public long Add(string AgencyNumber, CarReservation Entity)
        {
            try
            {
                using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                {
                    Entity.CreatedOn = DateTime.Now;
                    DB.CarReservations.Add(Entity);
                    DB.SaveChanges();
                    return Entity.Id;
                }
            }
            catch (Exception) {
                return -1;
            }
        }
        public bool Update(string AgencyNumber, CarReservation Entity)
        {
            try
            {
                using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                {
                    var entity = DB.CarReservations.Where(e => e.Id == Entity.Id).FirstOrDefault();
                    if (entity == null) return false;

                    entity.ModifiedBy = Entity.ModifiedBy;
                    entity.ModifiedOn = DateTime.Now;
                    entity.CancelledBy = Entity.CancelledBy;
                    entity.CancelledOn = Entity.CancelledOn;
                    entity.PaymentStatusId = Entity.PaymentStatusId;
                    entity.ClientId = Entity.ClientId;
                    entity.ReservationNumber = Entity.ReservationNumber;
                    entity.FlightNumber = Entity.FlightNumber;
                    entity.FromDate = Entity.FromDate;
                    entity.ToDate = Entity.ToDate;
                    entity.CarProviderId = Entity.CarProviderId;
                    entity.CarCategoryId = Entity.CarCategoryId;
                    entity.TourOperatorId = Entity.TourOperatorId;
                    entity.ProvinceId = Entity.ProvinceId;
                    entity.RentCarPlaceId = Entity.RentCarPlaceId;
                    entity.HasInsurance = Entity.HasInsurance;
                    entity.CostPrice = Entity.CostPrice;
                    entity.SalePrice = Entity.SalePrice;
                    entity.InsuranceCost = Entity.InsuranceCost;
                    entity.Discount = Entity.Discount;
                    entity.Note = Entity.Note;

                    DB.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(string AgencyNumber, int Id) {
            try
            {
                using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                {
                    var entity = DB.CarReservations.Where(e => e.Id == Id).FirstOrDefault();
                    if (entity == null) return true;

                    DB.CarReservations.Remove(entity);
                    DB.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public CarReservationsPerAgent GetReservationsPerMonthAndAgent(string AgencyNumber, int AgentId, int FirstYear, int SecondYear, int Month) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var firstyear = from cr in DB.CarReservations
                                where cr.CreatedOn.Year == FirstYear & cr.CreatedOn.Month == Month & cr.CreatedBy == AgentId & (cr.CreatedOn.Day % 3 == 0 | cr.CreatedOn.Day == 1)
                                select new { Day = cr.CreatedOn.Day };
                var secondyear = from cr in DB.CarReservations
                                where cr.CreatedOn.Year == SecondYear & cr.CreatedOn.Month == Month & cr.CreatedBy == AgentId & (cr.CreatedOn.Day % 3 == 0 | cr.CreatedOn.Day == 1)
                                 select new { Day = cr.CreatedOn.Day };

                //Create Object
                CarReservationsPerAgent data = new CarReservationsPerAgent();
                data.FirstYear = FirstYear;
                data.SecondYear = SecondYear;
                data.Month = Month;
                data.FirstYearReservations = new int[10];
                data.SecondYearReservations = new int[10];

                int[] days = new int[10] { 1, 3, 6, 9, 12, 15, 18, 21, 24, 27 };
                int index = 0;
                foreach (int day in days) {
                    data.FirstYearReservations[index] = 0;
                    data.SecondYearReservations[index] = 0;
                    foreach (var elem in firstyear)
                        if (elem.Day == day) data.FirstYearReservations[index] += 1;
                    foreach (var elem in secondyear)
                        if (elem.Day == day) data.SecondYearReservations[index] += 1;
                    index += 1;
                }

                return data;
            }
        }
        public List<CarReservationExtension> GetCarReservationsReport(string AgencyNumber, DateTime FromDate, DateTime ToDate) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from cr in DB.CarReservations
                        join c in DB.Clients on cr.ClientId equals c.Id
                        join cp in DB.CarProviders on cr.CarProviderId equals cp.Id into cproviders
                        from subpro in cproviders.DefaultIfEmpty()
                        join cc in DB.CarCategories on cr.CarCategoryId equals cc.Id into ccat
                        from subcat in ccat.DefaultIfEmpty()
                        join to in DB.TourOperators on cr.TourOperatorId equals to.Id into tope
                        from subtour in tope.DefaultIfEmpty()
                        join p in DB.Provinces on cr.ProvinceId equals p.Id into prov
                        from subprov in prov.DefaultIfEmpty()
                        join rcp in DB.RentCarPlaces on cr.RentCarPlaceId equals rcp.Id into ren
                        from subrent in ren.DefaultIfEmpty()
                        where cr.FromDate >= FromDate & cr.FromDate <= ToDate & cr.CancelledOn == null
                        select new CarReservationExtension
                        {
                            Id = cr.Id,
                            CreatedBy = cr.CreatedBy,
                            CreatedByUser = string.Empty,
                            CreatedOn = cr.CreatedOn,
                            ModifiedBy = cr.ModifiedBy,
                            ModifiedByUser = string.Empty,
                            ModifiedOn = cr.ModifiedOn,
                            CancelledOn = cr.CancelledOn,
                            CancelledBy = cr.CancelledBy,
                            CancelledByUser = string.Empty,
                            PaymentStatusId = cr.PaymentStatusId,
                            ClientId = cr.ClientId,
                            ClientFirstname = c.FirstName,
                            ClientLastname = c.LastName,
                            ClientPhone = c.Phone,
                            ClientEmail = c.Email,
                            TroubledClient = c.Troubled,
                            ReservationNumber = cr.ReservationNumber,
                            FlightNumber = cr.FlightNumber,
                            FromDate = cr.FromDate,
                            ToDate = cr.ToDate,
                            CarProviderId = cr.CarProviderId,
                            CarProviderName = subpro.Name,
                            CarCategoryId = cr.CarCategoryId,
                            CarCategoryName = subcat.Name,
                            TourOperatorId = cr.TourOperatorId,
                            TourOperatorName = subtour.Name,
                            ProvinceId = cr.ProvinceId,
                            ProvinceName = subprov.Name,
                            RentCarPlaceId = cr.RentCarPlaceId,
                            RentCarPlaceName = subrent.Name,
                            HasInsurance = cr.HasInsurance,
                            CostPrice = cr.CostPrice,
                            SalePrice = cr.SalePrice,
                            InsuranceCost = cr.InsuranceCost,
                            Discount = cr.Discount,
                            Note = cr.Note
                        };
                return query.OrderBy(e => e.FromDate).ToList();
            }
        }
    }
}
