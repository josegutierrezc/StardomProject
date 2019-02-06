using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppsManager.DL;
using AppsManager.DTO;
using Cars.DL;
using Cars.DTO;
using FluentValidation;
using AutoMapper;
using FluentValidation.Results;

namespace Cars.REST.Controllers
{
    public class ClientsController : ApiController
    {
        [Route("api/v1/agencies/{agencynumber}/clients")]
        [HttpGet]
        public HttpResponseMessage Clients(string agencynumber, int pagesize, int pagenumber, string searchfor)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientsManager cliMan = new ClientsManager();
            KeyValuePair<int, List<ClientExtension>> result = cliMan.GetAll(agencynumber, pagesize, pagenumber, searchfor);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, new KeyValuePair<int, List<ClientDTO>>(result.Key, Mapper.Map<List<ClientExtension>, List<ClientDTO>>(result.Value)));
        }

        [Route("api/v1/agencies/{agencynumber}/clients")]
        [HttpPost]
        public HttpResponseMessage ClientPost(string agencynumber, [FromBody] ClientDTO dto) {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientDTOValidator validator = new ClientDTOValidator();
            ValidationResult result = validator.Validate(dto);
            if (!result.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

            //Update CreatedBy
            UsersManager userMan = new UsersManager();
            dto.CreatedByUserId = userMan.GetByUsername(dto.CreatedByUsername).Id;

            ClientsManager cliMan = new ClientsManager();
            int id = cliMan.Add(agencynumber, Mapper.Map<ClientDTO, Client>(dto));

            return Request.CreateResponse(System.Net.HttpStatusCode.Created, id);
        }

        [Route("api/v1/agencies/{agencynumber}/clients/{id}")]
        [HttpGet]
        public HttpResponseMessage ClientGet(string agencynumber, int id)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientsManager cliMan = new ClientsManager();
            ClientExtension client = cliMan.GetById(agencynumber, id);
            return client == null ? Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Client does not exist.") : Request.CreateResponse(System.Net.HttpStatusCode.OK, Mapper.Map<ClientExtension, ClientDTO>(client));
        }

        [Route("api/v1/agencies/{agencynumber}/clients/{id}")]
        [HttpPut]
        public HttpResponseMessage ClientPut(string agencynumber, int id, [FromBody]ClientDTO dto)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientsManager cliMan = new ClientsManager();
            ClientExtension client = cliMan.GetById(agencynumber, id);
            if (client == null) Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Client does not exists.");

            UsersManager userMan = new UsersManager();
            dto.ModifiedByUserId = userMan.GetByUsername(dto.ModifiedByUsername).Id;

            bool updated = cliMan.Update(agencynumber, Mapper.Map<ClientDTO, ClientExtension>(dto));

            return updated ? Request.CreateResponse(System.Net.HttpStatusCode.OK) : Request.CreateResponse(System.Net.HttpStatusCode.BadGateway, "An error occurred trying to update this client.");
        }

        [Route("api/v1/agencies/{agencynumber}/clients/{id}")]
        [HttpDelete]
        public HttpResponseMessage ClientDelete(string agencynumber, int id)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientsManager cliMan = new ClientsManager();
            bool removed = cliMan.Remove(agencynumber, id);

            return removed ? Request.CreateResponse(System.Net.HttpStatusCode.OK) : Request.CreateResponse(System.Net.HttpStatusCode.BadGateway, "An error occurred trying to remove this client.");
        }

        [Route("api/v1/agencies/{agencynumber}/clients/{id}/statistics")]
        [HttpGet]
        public HttpResponseMessage ClientStatistics(string agencynumber, int id)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientsManager cliMan = new ClientsManager();
            ClientExtension client = cliMan.GetById(agencynumber, id);
            if (client == null) Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Client does not exists.");

            //Create Statistics object
            ClientStatisticDTO dto = new ClientStatisticDTO();
            dto.ClientId = client.Id;

            //Define membership
            dto.ClientSince = client.CreatedOn == null ? DateTime.MinValue : (DateTime)client.CreatedOn;

            CarReservationsManager resMan = new CarReservationsManager();
            List<CarReservationExtension> reservations = resMan.GetByClientId(agencynumber, id);

            //Initialize total reservations
            dto.TotalCarReservations = 0;

            Dictionary<int, int> dictYears = new Dictionary<int, int>();
            Dictionary<string, int> dictCategories = new Dictionary<string, int>();

            foreach (CarReservationExtension r in reservations) {
                if (r.CancelledOn != null | (r.ReservationNumber == null || r.ReservationNumber.Trim() == string.Empty)) continue;

                dto.TotalCarReservations += 1;
                r.ToDate = new DateTime(r.ToDate.Year, r.ToDate.Month, r.ToDate.Day, r.FromDate.Hour, r.FromDate.Minute, r.FromDate.Second);
                int totalDays = r.ToDate.Subtract(r.FromDate).Days;

                //Save total reservation days to use it later calculating the average
                dto.TotalCarReservationDays += totalDays;

                //Keep track of total of reservations per year for later calculate the average
                if (dictYears.ContainsKey(r.ToDate.Year))
                    dictYears[r.ToDate.Year] += 1;
                else
                    dictYears.Add(r.ToDate.Year, 1);

                //Keep track of car categories for later define which one was prefered
                if (dictCategories.ContainsKey(r.CarCategoryName))
                    dictCategories[r.CarCategoryName] += 1;
                else
                    dictCategories.Add(r.CarCategoryName, 1);
            }
            //Calculate average of car reservations days
            dto.AverageCarReservationDays = dto.TotalCarReservations == 0 ? 0 : dto.TotalCarReservationDays / dto.TotalCarReservations;

            //Calculate average of reservations per year
            var totalReservations = 0;
            foreach (KeyValuePair<int, int> kvp in dictYears) totalReservations += kvp.Value;
            dto.AverageCarReservationsPerYear = dictYears.Count() == 0 ? 0 : totalReservations / dictYears.Count();

            //Define three levels of car categories
            string preferedCategory = string.Empty;
            totalReservations = 0;
            foreach (KeyValuePair<string, int> kvp in dictCategories)
                if (kvp.Value > totalReservations) {
                    preferedCategory = kvp.Key;
                    totalReservations = kvp.Value;
                }
            dto.PreferedCarCategory = preferedCategory;

            return Request.CreateResponse(System.Net.HttpStatusCode.OK, dto);
        }

        [Route("api/v1/agencies/{agencynumber}/clients/{id}/carreservations")]
        [HttpGet]
        public HttpResponseMessage CarReservations(string agencynumber, int id)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientsManager cliMan = new ClientsManager();
            ClientExtension client = cliMan.GetById(agencynumber, id);
            if (client == null) Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Client does not exists.");

            CarReservationsManager resMan = new CarReservationsManager();
            List<CarReservationExtension> reservations = resMan.GetByClientId(agencynumber, id);
            UsersManager userMan = new UsersManager();
            foreach (CarReservationExtension r in reservations) {
                r.CreatedByUser = userMan.GetById(r.CreatedBy).Username;
                r.ModifiedByUser = r.ModifiedOn == null ? string.Empty : userMan.GetById((int)r.ModifiedBy).Username;
            }

            return Request.CreateResponse(System.Net.HttpStatusCode.OK, Mapper.Map<List<CarReservationExtension>, List<CarReservationDTO>>(reservations));
        }

        [Route("api/v1/agencies/{agencynumber}/linkedagencies")]
        [HttpGet]
        public HttpResponseMessage LinkedAgenciesGet(string agencynumber)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            LinkedAgenciesManager linkMan = new LinkedAgenciesManager();
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, Mapper.Map<List<LinkedAgency>, List<LinkedAgencyDTO>>(linkMan.Get(agencynumber)));
        }

        [Route("api/v1/agencies/{agencynumber}/linkedagencies")]
        [HttpPost]
        public HttpResponseMessage LinkedAgenciesPost(string agencynumber, [FromBody] LinkedAgencyDTO dto)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            LinkedAgenciesManager linkMan = new LinkedAgenciesManager();
            LinkedAgency entity = Mapper.Map<LinkedAgencyDTO, LinkedAgency>(dto);
            int added = linkMan.Add(agencynumber, entity);
            return added != -1 ? Request.CreateResponse(System.Net.HttpStatusCode.OK, added) : Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        }

        [Route("api/v1/agencies/{agencynumber}/linkedagencies")]
        [HttpPut]
        public HttpResponseMessage LinkedAgenciesPut(string agencynumber, [FromBody] LinkedAgencyDTO dto)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            LinkedAgenciesManager linkMan = new LinkedAgenciesManager();
            LinkedAgency entity = Mapper.Map<LinkedAgencyDTO, LinkedAgency>(dto);
            bool updated = linkMan.Update(agencynumber, entity);
            return updated ? Request.CreateResponse(System.Net.HttpStatusCode.OK) : Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        }

        #region Public Helpers
        [Route("api/v1/agencies/{agencynumber}/clients/helpers/formatphone")]
        [HttpGet]
        public HttpResponseMessage FormatClientsPhone(string agencynumber)
        {
            AgenciesManager ageMan = new AgenciesManager();
            Agency agency = ageMan.GetByNumber(agencynumber);
            if (agency == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Agency " + agencynumber + " does not exists.");

            ClientsManager cliMan = new ClientsManager();
            return cliMan.FormatPhoneNumber(agencynumber) ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        #endregion

    }
}
