using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using AppsManager.DL;
using AppsManager.DTO;
using Cars.DL;
using Cars.DTO;
using Cars.Reports;
using AutoMapper;
using System.Net.Http;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Net.Http.Headers;

namespace Cars.REST.Controllers
{
    public class AgenciesController : ApiController
    {
        #region Agencies
        
        [Route("api/v1/agencies")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<Agency>, IEnumerable<AgencyDTO>>(man.GetAll()));
            }
        }

        [Route("api/v1/agencies")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]AgencyDTO dto)
        {
            //Validate
            AgencyDTOValidator validator = new AgencyDTOValidator();
            ValidationResult result = validator.Validate(dto);
            if (!result.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                bool added = ageMan.Add(Mapper.Map<AgencyDTO, Agency>(dto));
                return added ? Request.CreateResponse(HttpStatusCode.Created) : Request.CreateResponse(HttpStatusCode.Conflict, "An Agency with number " + dto.Number + " exists already.");
            }
        }

        [Route("api/v1/agencies/{agencynumber}")]
        [HttpGet]
        public HttpResponseMessage Get(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");
                else
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, Mapper.Map<Agency, AgencyDTO>(agency));
            }
        }

        [Route("api/v1/agencies/{agencynumber}")]
        [HttpPut]
        public HttpResponseMessage Put(string agencynumber, [FromBody]AgencyDTO dto)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                if (ageMan.GetByNumber(agencynumber) == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                AgencyDTOValidator validator = new AgencyDTOValidator();
                ValidationResult result = validator.Validate(dto);
                if (!result.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

                Agency agency = Mapper.Map<AgencyDTO, Agency>(dto);
                bool updated = ageMan.Update(agencynumber, agency);
                return updated ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to update Agency " + agencynumber);
            }
        }

        [Route("api/v1/agencies/{agencynumber}")]
        [HttpDelete]
        public HttpResponseMessage Delete(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                bool deleted = ageMan.Delete(agencynumber);
                return deleted ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to delete Agency " + agencynumber);
            }
        }

        #endregion

        #region CarReservations

        [Route("api/v1/agencies/{agencynumber}/carreservations")]
        [HttpGet]
        public HttpResponseMessage CarReservations(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                return Request.CreateResponse(HttpStatusCode.OK, "Too much data could be returned without parameters specifications. URL should looks like: api/v1/agencies/{agencynumber}/carreservations?type=[type]&pagesize=[pagesize]&pagenumber=[pagenumber]&searchfor=[searchfor]. Where [type] could be active or history, [pagesize] is an integer indicating total records per page returned, [pagenumber] is an integer indicating the page number you want to get, every page with size [pagesize] and [searchfor] is an string which could be contained in fields 'client firstname, client lastname, client phone, reservation number");
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations")]
        [HttpGet]
        public HttpResponseMessage CarReservations(string agencynumber, [FromUri]string type, [FromUri]int pagesize, [FromUri]int pagenumber, [FromUri]string searchfor, [FromUri]string filter)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                //Get All reservations that meet this requirements
                CarReservationsManager carMan = new CarReservationsManager();
                KeyValuePair<int, List<CarReservationExtension>> reservations = carMan.Get(agencynumber, type, searchfor, pagesize, pagenumber, filter);

                //Update Total days, and user names
                UsersManager userMan = new UsersManager();
                foreach (CarReservationExtension res in reservations.Value)
                {
                    res.Days = res.ToDate.Subtract(res.FromDate).Days;
                    res.CreatedByUser = userMan.GetById(res.CreatedBy).Username;
                    res.ModifiedByUser = res.ModifiedBy == null ? string.Empty : userMan.GetById((int)res.ModifiedBy).Username;
                    res.CancelledByUser = res.CancelledBy == null ? string.Empty : userMan.GetById((int)res.CancelledBy).Username;
                }

                List<CarReservationDTO> reservationsDTO = Mapper.Map<List<CarReservationExtension>, List<CarReservationDTO>>(reservations.Value);
                return Request.CreateResponse(HttpStatusCode.OK, new KeyValuePair<int, List<CarReservationDTO>>(reservations.Key, reservationsDTO));
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations")]
        [HttpPost]
        public HttpResponseMessage ReservationPost(string agencynumber, [FromBody] CarReservationDTO dto)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                //Validate
                CarReservationDTOValidation validator = new CarReservationDTOValidation();
                ValidationResult result = validator.Validate(dto);
                if (!result.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

                //Update UserId
                UsersManager userMan = new UsersManager();
                dto.CreatedBy = userMan.GetByUsername(dto.CreatedByUser).Id;

                //Add reservation
                CarReservationsManager carMan = new CarReservationsManager();
                long id = carMan.Add(agencynumber, Mapper.Map<CarReservationDTO, CarReservation>(dto));

                if (id == -1)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to add this reservation to the database.");

                return Request.CreateResponse(HttpStatusCode.Created, id);
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations/{id}")]
        [HttpGet]
        public HttpResponseMessage ReservationGet(string agencynumber, int id)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                //Get reservation
                CarReservationsManager carMan = new CarReservationsManager();
                CarReservationExtension reservation = carMan.GetById(agencynumber, id);
                if (reservation == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Reservation with Id " + id + " does not exists.");

                //Update Total days, and user names
                UsersManager userMan = new UsersManager();
                reservation.Days = reservation.ToDate.Subtract(reservation.FromDate).Days;
                reservation.CreatedByUser = userMan.GetById(reservation.CreatedBy).Username;
                reservation.ModifiedByUser = reservation.ModifiedBy == null ? string.Empty : userMan.GetById((int)reservation.ModifiedBy).Username;
                reservation.CancelledByUser = reservation.CancelledBy == null ? string.Empty : userMan.GetById((int)reservation.CancelledBy).Username;

                CarReservationDTO reservationDTO = Mapper.Map<CarReservationExtension, CarReservationDTO>(reservation);
                return Request.CreateResponse(HttpStatusCode.OK, reservationDTO);
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations/{id}")]
        [HttpPut]
        public HttpResponseMessage ReservationPut(string agencynumber, int id, [FromBody] CarReservationDTO dto)
        {
            try
            {
                using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                    AgenciesManager ageMan = new AgenciesManager(DB);
                    Agency agency = ageMan.GetByNumber(agencynumber);
                    if (agency == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                    if (dto == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "No data was returned from message body.");

                    //Validate
                    CarReservationDTOValidation validator = new CarReservationDTOValidation();
                    ValidationResult result = validator.Validate(dto);
                    if (!result.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

                    //Get UserId
                    UsersManager userMan = new UsersManager();
                    dto.ModifiedBy = userMan.GetByUsername(dto.ModifiedByUser).Id;
                    if (dto.CancelledByUser != null && dto.CancelledByUser != string.Empty)
                        dto.CancelledBy = userMan.GetByUsername(dto.CancelledByUser).Id;

                    //Update PaymentStatusId
                    dto.PaymentStatusId = GetReservationPaymentsStatusId(agencynumber, dto.Id, dto.FromDate, dto.ToDate, dto.SalePrice == null ? 0 : (double)dto.SalePrice, dto.Discount == null ? 0 : (double)dto.Discount);

                    //Update reservation
                    dto.Id = id;
                    CarReservationsManager carMan = new CarReservationsManager();
                    bool updated = carMan.Update(agencynumber, Mapper.Map<CarReservationDTO, CarReservation>(dto));

                    return updated ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to update this reservation.");
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations/{id}")]
        [HttpDelete]
        public HttpResponseMessage ReservationDelete(string agencynumber, int id)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                //Delete reservation
                CarReservationsManager carMan = new CarReservationsManager();
                bool deleted = carMan.Delete(agencynumber, id);

                return deleted ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to delete this reservation.");
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations/priceconfiguration")]
        [HttpGet]
        public HttpResponseMessage PriceConfiguration(string agencynumber, [FromUri]int carcategoryid, [FromUri]int touroperatorid, [FromUri]string fromdate, [FromUri]string todate)
        {
            DateTime fromDate = Convert.ToDateTime(fromdate);
            DateTime toDate = Convert.ToDateTime(todate);

            int numberOfDays = toDate.Subtract(fromDate).Days;
            int fromDayNumber = fromDate.Month * 100 + fromDate.Day;
            int toDayNumber = toDate.Month * 100 + toDate.Day;
            int seasonId = -1;
            int reservationDayId = -1;

            //Try to find a season
            if (fromDate.Year == toDate.Year)
            {
                SeasonsManager seaMan = new SeasonsManager();
                SeasonDate sd = seaMan.Get(agencynumber, fromDayNumber, toDayNumber);
                if (sd != null) seasonId = sd.SeasonId;
            }

            //Try to find reservation day
            ReservationDaysManager resMan = new ReservationDaysManager();
            ReservationDay reservationDay = resMan.Get(agencynumber, carcategoryid, numberOfDays);
            if (reservationDay != null) reservationDayId = reservationDay.Id;

            //If no season was found
            if (seasonId == -1)
            {
                SeasonsManager seaMan = new SeasonsManager();
                seasonId = seaMan.GetMax(agencynumber, fromDayNumber).SeasonId;
            }

            //if no reservation day was found
            if (reservationDayId == -1)
            {
                //IMPORTANT: This method fail if no records exist in CarCategoryReservationDays pointing to ReservationDays with more than 29 days. 
                //To fix it ReservationDays with up to 29 days can only exist.
                ReservationDay rd = resMan.GetMax(agencynumber, carcategoryid);
                if (rd == null) return Request.CreateResponse(HttpStatusCode.OK);
                reservationDayId = rd.Id;
            }

            PriceConfigurationsManager pcMan = new PriceConfigurationsManager();
            PriceConfiguration pc = pcMan.Get(agencynumber, carcategoryid, touroperatorid, seasonId, reservationDayId);

            if (pc == null) return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<PriceConfiguration, PriceConfigurationDTO>(pc));
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations/{id}/payments")]
        [HttpGet]
        public HttpResponseMessage ReservationPaymentsGet(string agencynumber, int id)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                //Get Agency
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                //Get CarReservation
                CarReservationsManager resMan = new CarReservationsManager();
                CarReservationExtension reservation = resMan.GetById(agencynumber, id);
                if (reservation == null) Request.CreateResponse(HttpStatusCode.BadRequest, "Car Reservation does not exists.");

                //Return Payments
                UsersManager userMan = new UsersManager();
                CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
                List<PaymentExtension> payments = payMan.Get(agencynumber, id);
                foreach (PaymentExtension p in payments)
                {
                    p.CreatedByUser = userMan.GetById(p.CreatedByUserId).Username;
                }

                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<PaymentExtension>, List<CarReservationPaymentDTO>>(payments));
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations/{id}/payments")]
        [HttpPost]
        public HttpResponseMessage ReservationPaymentsPost(string agencynumber, int id, [FromBody]CarReservationPaymentDTO paymentdto)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                //Get Agency
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                //Get CarReservation
                CarReservationsManager resMan = new CarReservationsManager();
                CarReservationExtension reservation = resMan.GetById(agencynumber, id);
                if (reservation == null) Request.CreateResponse(HttpStatusCode.BadRequest, "Car Reservation does not exists.");

                //Update userid
                UsersManager userMan = new UsersManager();
                paymentdto.CreatedByUserId = userMan.GetByUsername(paymentdto.CreatedByUser).Id;

                //Validate
                CarReservationPaymentDTOValidator validator = new CarReservationPaymentDTOValidator();
                ValidationResult result = validator.Validate(paymentdto);
                if (!result.IsValid) Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

                //Convert
                Payment payment = Mapper.Map<CarReservationPaymentDTO, Payment>(paymentdto);

                //Add payment
                CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
                bool added = payMan.Add(agencynumber, payment);

                //Update Reservation Payment Status
                reservation.PaymentStatusId = GetReservationPaymentsStatusId(agencynumber, reservation.Id, reservation.FromDate, reservation.ToDate, reservation.SalePrice == null ? 0 : (double)reservation.SalePrice, reservation.Discount == null ? 0 : (double)reservation.Discount);
                bool updated = resMan.Update(agencynumber, Mapper.Map<CarReservationExtension, CarReservation>(reservation));

                return added ? Request.CreateResponse(HttpStatusCode.Created) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred adding this payment.");
            }
        }

        [Route("api/v1/agencies/{agencynumber}/carreservations/{id}/payments/{paymentid}")]
        [HttpDelete]
        public HttpResponseMessage ReservationPaymentsDelete(string agencynumber, int id, int paymentid)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                //Get Agency
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                //Get CarReservation
                CarReservationsManager resMan = new CarReservationsManager();
                CarReservationExtension reservation = resMan.GetById(agencynumber, id);
                if (reservation == null) Request.CreateResponse(HttpStatusCode.BadRequest, "Car Reservation does not exists.");

                //Delete payment
                CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
                bool deleted = payMan.Delete(agencynumber, paymentid);

                //Update Reservation Payment Status
                reservation.PaymentStatusId = GetReservationPaymentsStatusId(agencynumber, reservation.Id, reservation.FromDate, reservation.ToDate, reservation.SalePrice == null ? 0 : (double)reservation.SalePrice, reservation.Discount == null ? 0 : (double)reservation.Discount);
                bool updated = resMan.Update(agencynumber, Mapper.Map<CarReservationExtension, CarReservation>(reservation));

                return deleted ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred removing this payment.");
            }
        }
        #endregion

        #region PaymentConcepts
        [Route("api/v1/agencies/{agencynumber}/paymentconcepts")]
        [HttpGet]
        public HttpResponseMessage PaymentConceptsGet(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                PaymentConceptsManager payMan = new PaymentConceptsManager();
                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<PaymentConcept>, List<PaymentConceptDTO>>(payMan.Get(agencynumber)));
            }
        }
        #endregion

        #region PaymentMethods
        [Route("api/v1/agencies/{agencynumber}/paymentmethods")]
        [HttpGet]
        public HttpResponseMessage PaymentMethodsGet(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                PaymentMethodsManager payMan = new PaymentMethodsManager();
                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<PaymentMethod>, List<PaymentMethodDTO>>(payMan.Get(agencynumber)));
            }
        }
        #endregion

        #region Users

        [Route("api/v1/agencies/{agencynumber}/users")]
        [HttpGet]
        public HttpResponseMessage Users(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                UsersManager userMan = new UsersManager();
                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<UserAgencyHelper>, List<UserAgencyDTO>>(userMan.GetUsers(agencynumber)));
            }
        }

        [Route("api/v1/agencies/{agencynumber}/users/{username}")]
        [HttpGet]
        public HttpResponseMessage UserGet(string agencynumber, string username)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                UsersManager userMan = new UsersManager();
                User user = userMan.GetByUsername(username);

                if (user == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "User " + username + " does not exists.");

                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<User, UserDTO>(user));
            }
        }

        [Route("api/v1/agencies/{agencynumber}/users/{username}/privileges")]
        [HttpGet]
        public HttpResponseMessage Privileges(string agencynumber, string username)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                Agency agency = ageMan.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                UsersManager userMan = new UsersManager();
                User user = userMan.GetByUsername(username);

                if (user == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "User " + username + " does not exists.");

                using (var CarsDB = CarsModel.ConnectToSqlServer(agencynumber))
                {
                    PrivilegesManager priMan = new PrivilegesManager(CarsDB);
                    List<Privilege> privileges = priMan.GetByUser(user.Id);

                    return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<Privilege>, List<PrivilegeDTO>>(privileges));
                }
            }
        }

        [Route("api/v1/agencies/{agencynumber}/users/{username}/privileges")]
        [HttpPost]
        public HttpResponseMessage AddPrivilege(string agencynumber, string username, [FromBody] UserPrivilegeDTO dto)
        {
            //Verify no privileges can be added to sysadmin account
            if (username.ToUpper() == "SYSADMIN") return Request.CreateResponse(HttpStatusCode.Forbidden, "No more privileges can be added to the System Administrator account.");

            //Validate
            UserPrivilegeDTOValidator validator = new UserPrivilegeDTOValidator();
            ValidationResult result = validator.Validate(dto);
            if (!result.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);
            if (username.ToUpper() != dto.Username.ToUpper()) return Request.CreateResponse(HttpStatusCode.BadRequest, "Username in URL does not match with Username in message body.");

            //Get User
            UsersManager userMan = new UsersManager();
            User user = userMan.GetByUsername(dto.Username);
            if (user == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "User does not exist.");

            //Assign Privilege
            using (var CarsDB = CarsModel.ConnectToSqlServer(agencynumber)) {
                PrivilegesManager priMan = new PrivilegesManager(CarsDB);
                bool assigned = priMan.AssignToUser(user.Id, dto.PrivilegeName);
                return assigned ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to assign this privilege to the user " + dto.Username);
            }
        }

        [Route("api/v1/agencies/{agencynumber}/users/{username}/privileges/{privilegename}")]
        [HttpDelete]
        public HttpResponseMessage DeletePrivilege(string agencynumber, string username, string privilegename)
        {
            //Verify no privileges can be added to sysadmin account
            if (username.ToUpper() == "SYSADMIN") return Request.CreateResponse(HttpStatusCode.Forbidden, "No privileges can be removed from the System Administrator account.");
            
            //Get User
            UsersManager userMan = new UsersManager();
            User user = userMan.GetByUsername(username);
            if (user == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "User does not exist.");

            //Assign Privilege
            using (var CarsDB = CarsModel.ConnectToSqlServer(agencynumber)) {
                PrivilegesManager priMan = new PrivilegesManager(CarsDB);
                bool unassigned = priMan.UnassignFromUser(user.Id, privilegename);
                return unassigned ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to unassign this privilege from the user " + username);
            }
        }

        #endregion

        #region CarCategories
        [Route("api/v1/agencies/{agencynumber}/carcategories")]
        [HttpGet]
        public HttpResponseMessage CarCategories(string agencynumber) {
            CarCategoriesManager carMan = new CarCategoriesManager();
            List<CarCategory> categories = carMan.Get(agencynumber);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<CarCategory>, List<CarCategoryDTO>>(categories));
        }
        [Route("api/v1/agencies/{agencynumber}/carcategories")]
        [HttpGet]
        public HttpResponseMessage CarCategories(string agencynumber, [FromUri]int carproviderid)
        {
            CarCategoriesManager carMan = new CarCategoriesManager();
            List<CarCategory> categories = carMan.Get(agencynumber, carproviderid);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<CarCategory>, List<CarCategoryDTO>>(categories));
        }
        #endregion

        #region Provinces
        [Route("api/v1/agencies/{agencynumber}/provinces")]
        [HttpGet]
        public HttpResponseMessage Provinces(string agencynumber)
        {
            ProvincesManager proMan = new ProvincesManager();
            List<Province> entitites = proMan.Get(agencynumber);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<Province>, List<ProvinceDTO>>(entitites));
        }
        #endregion

        #region RentCarPlaces
        [Route("api/v1/agencies/{agencynumber}/rentcarplace")]
        [HttpGet]
        public HttpResponseMessage RentCarPlace(string agencynumber, [FromUri]int provinceid, [FromUri]bool onlyactives)
        {
            RentCarPlacesManager man = new RentCarPlacesManager();
            List<RentCarPlace> entitites = onlyactives ? man.GetActives(agencynumber, provinceid) : man.Get(agencynumber, provinceid);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<RentCarPlace>, List<RentCarPlaceDTO>>(entitites));
        }
        #endregion

        #region CarProviders
        [Route("api/v1/agencies/{agencynumber}/carproviders")]
        [HttpGet]
        public HttpResponseMessage CarProviders(string agencynumber)
        {
            CarProvidersManager carMan = new CarProvidersManager();
            List<CarProvider> providers = carMan.Get(agencynumber);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<CarProvider>, List<CarProviderDTO>>(providers));
        }
        #endregion

        #region TourOperators
        [Route("api/v1/agencies/{agencynumber}/touroperators")]
        [HttpGet]
        public HttpResponseMessage TourOperators(string agencynumber, [FromUri]bool onlyactives)
        {
            TourOperatorsManager tourMan = new TourOperatorsManager();
            List<TourOperator> operators = tourMan.Get(agencynumber, onlyactives);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<TourOperator>, List<TourOperatorDTO>>(operators));
        }

        [Route("api/v1/agencies/{agencynumber}/touroperators")]
        [HttpGet]
        public HttpResponseMessage TourOperators(string agencynumber, [FromUri]int carproviderid)
        {
            TourOperatorsManager tourMan = new TourOperatorsManager();
            List<TourOperator> operators = tourMan.Get(agencynumber, carproviderid);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<TourOperator>, List<TourOperatorDTO>>(operators));
        }

        [Route("api/v1/agencies/{agencynumber}/touroperators/{id}")]
        [HttpGet]
        public HttpResponseMessage TourOperatorGet(string agencynumber, int id) {
            TourOperatorsManager tourMan = new TourOperatorsManager();
            TourOperator entity = tourMan.GetById(agencynumber, id);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<TourOperator, TourOperatorDTO>(entity));
        }
        [Route("api/v1/agencies/{agencynumber}/touroperators/{id}/carcategories")]
        [HttpGet]
        public HttpResponseMessage TourOperatorCarCategoriesGet(string agencynumber, int id)
        {
            TourOperatorsManager tourMan = new TourOperatorsManager();
            List<CarCategory> entities = tourMan.GetCarCategories(agencynumber, id);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<CarCategory>, List<CarCategoryDTO>>(entities));
        }
        #endregion

        #region Reports
        [Route("api/v1/agencies/{agencynumber}/reports")]
        [HttpGet]
        public HttpResponseMessage ReportsGet(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");
                else
                {
                    List<string> reportnames = new List<string>();
                    foreach (ReportModel report in ReportModel.All)
                        reportnames.Add(report.Url);
                    return Request.CreateResponse(HttpStatusCode.OK, reportnames);
                }
            }
        }

        [Route("api/v1/agencies/{agencynumber}/reports/{reportname}")]
        [HttpGet]
        public HttpResponseMessage ReportGet(string agencynumber, string reportname, string format, string parameters)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                foreach (ReportModel report in ReportModel.All)
                {
                    if (report.Url == reportname)
                    {
                        report.Initialize(agencynumber, format, parameters);
                        return Request.CreateResponse(HttpStatusCode.OK, report.Generate());
                    }
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Report was not found");
            }
        }
        #endregion

        #region PaymentStatuses
        [Route("api/v1/agencies/{agencynumber}/paymentstatuses")]
        [HttpGet]
        public HttpResponseMessage PaymentStatusesGet(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");
                PaymentStatusesManager payMan = new PaymentStatusesManager();
                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<PaymentStatus>, List<PaymentStatusDTO>>(payMan.Get(agencynumber)));
            }
        }
        #endregion

        #region Seasons
        [Route("api/v1/agencies/{agencynumber}/seasons")]
        [HttpGet]
        public HttpResponseMessage Seasons(string agencynumber)
        {
            SeasonsManager seaMan = new SeasonsManager();
            List<Season> seasons = seaMan.GetAll(agencynumber);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<Season>, List<SeasonDTO>>(seasons));
        }
        #endregion

        #region ReservationDays
        [Route("api/v1/agencies/{agencynumber}/reservationdays")]
        [HttpGet]
        public HttpResponseMessage ReservationDays(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                ReservationDaysManager dayMan = new ReservationDaysManager();
                List<ReservationDay> entities = dayMan.GetAll(agencynumber);

                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<ReservationDay>, List<ReservationDayDTO>>(entities));
            }
        }
        #endregion

        #region PriceConfiguration
        [Route("api/v1/agencies/{agencynumber}/priceconfigurations")]
        [HttpGet]
        public HttpResponseMessage PriceConfigurationGet(string agencynumber, [FromUri]int touroperatorid, [FromUri]int seasonid) {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                PriceConfigurationsManager priMan = new PriceConfigurationsManager();
                List<PriceConfiguration> entities = priMan.GetByTourOperatorAndSeason(agencynumber, touroperatorid, seasonid);

                return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<PriceConfiguration>, List<PriceConfigurationDTO>>(entities));
            }
        }

        [Route("api/v1/agencies/{agencynumber}/priceconfiguration")]
        [HttpPut]
        public HttpResponseMessage PriceConfigurationPut(string agencynumber, [FromUri]int touroperatorid, [FromUri]int seasonid, [FromUri]string ModifiedByUser, [FromBody]List<PriceConfigurationDTO> configuration)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                UsersManager userMan = new UsersManager();
                int userId = userMan.GetByUsername(ModifiedByUser).Id;

                PriceConfigurationsManager priMan = new PriceConfigurationsManager();
                bool updated = priMan.Update(agencynumber, touroperatorid, seasonid, userId, Mapper.Map<List<PriceConfigurationDTO>, List<PriceConfiguration>>(configuration));

                return updated ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region Privileges
        [Route("api/v1/agencies/{agencynumber}/privileges")]
        [HttpGet]
        public HttpResponseMessage GetAllPrivileges(string agencynumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
            {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                using (var CarsDB = CarsModel.ConnectToSqlServer(agencynumber))
                {
                    PrivilegesManager priMan = new PrivilegesManager(CarsDB);
                    return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<Privilege>, List<PrivilegeDTO>>(priMan.GetAll()));
                }
            }
        }
        #endregion

        #region Statistics
        [Route("api/v1/agencies/{agencynumber}/statistics")]
        [HttpGet]
        public HttpResponseMessage StatisticsGet(string agencynumber, [FromUri]string dataname, [FromUri]string parameters)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager man = new AgenciesManager(DB);
                Agency agency = man.GetByNumber(agencynumber);
                if (agency == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

                if (dataname.ToLower() == "carreservationsperagent")
                {
                    int firstyear = 0;
                    int secondyear = 0;
                    int month = 0;
                    string username = string.Empty;

                    string[] paramArray = parameters.Split(';');
                    foreach (string param in paramArray)
                    {
                        string[] kvp = param.Split('=');
                        if (kvp[0].ToLower() == "firstyear") firstyear = Convert.ToInt32(kvp[1]);
                        if (kvp[0].ToLower() == "secondyear") secondyear = Convert.ToInt32(kvp[1]);
                        if (kvp[0].ToLower() == "month") month = Convert.ToInt32(kvp[1]);
                        if (kvp[0].ToLower() == "username") username = kvp[1];
                    }

                    if (firstyear == 0 | secondyear == 0 | month == 0 | username == string.Empty)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Parameters: firstyear,secondyear,month and username are required.");

                    UsersManager userMan = new UsersManager();
                    int userId = userMan.GetByUsername(username).Id;

                    CarReservationsManager carMan = new CarReservationsManager();
                    CarReservationsPerAgent data = carMan.GetReservationsPerMonthAndAgent(agencynumber, userId, firstyear, secondyear, month);

                    return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<CarReservationsPerAgent, CarReservationsPerAgentDTO>(data));
                }
                else if (dataname.ToLower() == "paymentsbydate")
                {
                    DateTime fromDate = DateTime.MinValue;
                    int totalDays = 0;
                    int comparedWithYear = 0;

                    string[] paramArray = parameters.Split(';');
                    foreach (string param in paramArray)
                    {
                        string[] kvp = param.Split('=');
                        if (kvp[0].ToLower() == "fromdate") fromDate = kvp[1] == null | kvp[1] == string.Empty ? DateTime.MinValue : Convert.ToDateTime(kvp[1]);
                        if (kvp[0].ToLower() == "totaldays") totalDays = Convert.ToInt32(kvp[1]);
                        if (kvp[0].ToLower() == "comparedwithyear") comparedWithYear = Convert.ToInt32(kvp[1]);
                    }

                    if (fromDate == DateTime.MinValue | totalDays == 0 | comparedWithYear == 0)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Parameters: fromdate, totaldays, comparedwithyear are required.");

                    CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
                    PaymentsPerPeriod data = payMan.GetTotalPaymentsPerPeriodComparison(agencynumber, fromDate, totalDays, comparedWithYear);

                    return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<PaymentsPerPeriod, PaymentsPerPeriodDTO>(data));
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Data name is required.");
            }
        }
        #endregion

        #region Helpers
        private int GetReservationPaymentsStatusId(string AgencyNumber, long CarReservationId, DateTime FromDate, DateTime ToDate, double SalePrice, double Discount) {
            ToDate = new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, FromDate.Hour, FromDate.Minute, FromDate.Second);
            int totalDays = ToDate.Subtract(FromDate).Days;
            double finalSalePrice = SalePrice * totalDays - Discount * totalDays;

            CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
            double totalPaid = payMan.GetTotalPaid(AgencyNumber, CarReservationId);

            if (totalPaid <= 0) return 1;
            if (totalPaid > finalSalePrice) return 4;
            if (totalPaid == finalSalePrice) return 3;
            return 2;
        }
        #endregion
    }
}
