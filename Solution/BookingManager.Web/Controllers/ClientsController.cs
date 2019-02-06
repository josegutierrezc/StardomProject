using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BookingManager.Web.Helpers;
using BookingManager.Web.Models;
using Cars.DTO;
using Cars.REST.Client;

namespace BookingManager.Web.Controllers
{
    public class ClientsController : Controller
    {
        // GET: Clients
        public async Task<ActionResult> Index(int AssignToReservationId)
        {
            ClientsLogViewModel model = new ClientsLogViewModel();
            model.AgencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            model.AssignToReservationId = AssignToReservationId;
            model.PageSize = 150;
            model.PageNumber = 1;
            model.SearchFor = string.Empty;

            KeyValuePair<int, List<ClientDTO>> result = await Client.Instance.GetClients(model.AgencyNumber, model.PageSize, model.PageNumber, model.SearchFor);
            model.TotalResults = result.Key;
            model.TotalPages = result.Key % model.PageSize == 0 ? result.Key / model.PageSize : result.Key / model.PageSize + 1;
            model.Clients = result.Value;

            return View(model);
        }

        public async Task<ActionResult> Get(long AssignToReservationId, string PageSize, string PageNumber, string SearchFor)
        {
            ClientsLogViewModel model = new ClientsLogViewModel();
            model.AgencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            model.AssignToReservationId = AssignToReservationId;
            model.PageSize = Convert.ToInt32(PageSize);
            model.PageNumber = Convert.ToInt32(PageNumber);
            model.SearchFor = SearchFor;

            KeyValuePair<int, List<ClientDTO>> result = await Client.Instance.GetClients(model.AgencyNumber, model.PageSize, model.PageNumber, model.SearchFor);
            model.TotalResults = result.Key;
            model.TotalPages = result.Key % model.PageSize == 0 ? result.Key / model.PageSize : result.Key / model.PageSize + 1;
            model.Clients = result.Value;

            return PartialView("_Clients", model);
        }

        public async Task<ActionResult> GetDetails(long AssignToReservationId, int ClientId)
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            ClientDTO client = await Client.Instance.GetClientById(agencyNumber, ClientId);

            ClientViewModel model = new ClientViewModel();
            model.ReturnUrl = string.Empty;
            model.Id = client.Id;
            model.CreatedByUserId = client.CreatedByUserId;
            model.CreatedOn = client.CreatedOn;
            model.ModifiedByUserId = client.ModifiedByUserId;
            model.ModifiedOn = client.ModifiedOn;
            model.FirstName = client.FirstName;
            model.LastName = client.LastName;
            model.Phone = client.Phone == null || client.Phone.Trim() == string.Empty ? string.Empty : ApplicationHelper.Instance.FormatPhoneNumber(client.Phone, false).Value;
            model.Email = client.Email == null || client.Email.Trim() == string.Empty ? string.Empty : client.Email;
            model.LinkedToAgencyId = client.LinkedToAgencyId;
            model.LinkedAgencyName = client.LinkedAgencyName;
            model.Birthday = client.Birthday == null ? "Cumpleaños no especificado" : ((DateTime)client.Birthday).ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
            model.Notes = client.Notes == null || client.Notes.Trim() == string.Empty ? string.Empty : client.Notes.Trim();
            model.Troubled = client.Troubled;

            ClientStatisticDTO statistics = await Client.Instance.GetClientStatistics(agencyNumber, client.Id);
            model.ClientSince = statistics.ClientSince == null ? "No tenemos récord" : ((DateTime)statistics.ClientSince).ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
            model.TotalCarReservations = statistics.TotalCarReservations;
            model.TotalCarReservationDays = statistics.TotalCarReservationDays;
            model.AverageCarReservationDays = statistics.AverageCarReservationDays;
            model.AverageCarReservationsPerYear = statistics.AverageCarReservationsPerYear;
            model.PreferedCarCategory = statistics.PreferedCarCategory;

            model.Activities = new List<ClientActivityModel>();
            List<CarReservationDTO> reservations = await Client.Instance.GetClientCarReservations(agencyNumber, ClientId);
            for (int i = reservations.Count() - 1; i >= 0; i--)
            {
                ClientActivityModel activity = new ClientActivityModel();
                CarReservationDTO res = reservations[i];
                res.ToDate = new DateTime(res.ToDate.Year, res.ToDate.Month, res.ToDate.Day, res.FromDate.Hour, res.FromDate.Minute, res.FromDate.Second);

                activity.IsCarReservationActivity = true;
                activity.CarReservationCarCategory = res.CarCategoryName;
                activity.Cancelled = res.CancelledOn != null;
                activity.CarReservationCreatedByUser = res.CreatedByUser;
                activity.CarReservationCreatedOn = res.CreatedOn == null ? "No tenemos record" : ((DateTime)res.FromDate).ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper(); ;
                activity.CarReservationDiscount = res.Discount == null ? 0 : (double)res.Discount;
                activity.CarReservationFromDate = res.FromDate.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
                activity.CarReservationNote = res.Note;
                activity.CarReservationNumber = res.ReservationNumber;
                activity.CarReservationRentCarPlace = res.RentCarPlaceName;
                activity.CarReservationSalePrice = res.SalePrice == null ? 0 : (double)res.SalePrice;
                activity.CarReservationToDate = res.ToDate.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
                activity.CarReservationTotalDays = res.ToDate.Subtract(res.FromDate).Days;
                model.Activities.Add(activity);
            }

            ClientsLogViewModel logmodel = new ClientsLogViewModel();
            logmodel.AssignToReservationId = AssignToReservationId;
            logmodel.SelectedClient = model;

            return PartialView("_ClientDetails", logmodel);
        }

        public async Task<ActionResult> View(int Id, string ReturnUrl) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            ClientDTO client = await Client.Instance.GetClientById(agencyNumber, Id);

            ClientViewModel model = new ClientViewModel();
            model.ReturnUrl = ReturnUrl;
            model.Id = client.Id;
            model.CreatedByUserId = client.CreatedByUserId;
            model.CreatedOn = client.CreatedOn;
            model.ModifiedByUserId = client.ModifiedByUserId;
            model.ModifiedOn = client.ModifiedOn;
            model.FirstName = client.FirstName;
            model.LastName = client.LastName;
            model.Phone = client.Phone == null || client.Phone.Trim() == string.Empty ? string.Empty : client.Phone;
            model.Email = client.Email == null || client.Email.Trim() == string.Empty ? string.Empty : client.Email;
            model.LinkedToAgencyId = client.LinkedToAgencyId;
            model.LinkedAgencyName = client.LinkedAgencyName;
            model.Birthday = client.Birthday == null ? "Cumpleaños no especificado" : ((DateTime)client.Birthday).ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
            model.Notes = client.Notes;
            model.Troubled = client.Troubled;

            ClientStatisticDTO statistics = await Client.Instance.GetClientStatistics(agencyNumber, client.Id);
            model.ClientSince = statistics.ClientSince == null ? "No tenemos récord" : ((DateTime)statistics.ClientSince).ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
            model.TotalCarReservations = statistics.TotalCarReservations;
            model.TotalCarReservationDays = statistics.TotalCarReservationDays;
            model.AverageCarReservationDays = statistics.AverageCarReservationDays;
            model.AverageCarReservationsPerYear = statistics.AverageCarReservationsPerYear;
            model.PreferedCarCategory = statistics.PreferedCarCategory;

            model.Activities = new List<ClientActivityModel>();
            List<CarReservationDTO> reservations = await Client.Instance.GetClientCarReservations(agencyNumber, Id);
            for (int i = reservations.Count() - 1; i >= 0; i--) {
                ClientActivityModel activity = new ClientActivityModel();
                CarReservationDTO res = reservations[i];
                activity.IsCarReservationActivity = true;
                activity.CarReservationCarCategory = res.CarCategoryName;
                activity.Cancelled = res.CancelledOn != null;
                activity.CarReservationCreatedByUser = res.CreatedByUser;
                activity.CarReservationCreatedOn = res.CreatedOn == null ? "No tenemos record" : ((DateTime)res.CreatedOn).ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
                activity.CarReservationDiscount = res.Discount == null ? 0 : (double)res.Discount;
                activity.CarReservationFromDate = res.FromDate.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
                activity.CarReservationNote = res.Note;
                activity.CarReservationNumber = res.ReservationNumber;
                activity.CarReservationRentCarPlace = res.RentCarPlaceName;
                activity.CarReservationSalePrice = res.SalePrice == null ? 0 : (double)res.SalePrice;
                activity.CarReservationToDate = res.ToDate.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
                model.Activities.Add(activity);
            }
            
            return View(model);
        }

        public async Task<ActionResult> Add() {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            ClientViewModel model = new ClientViewModel();

            List<LinkedAgencyDTO> agencies = await Client.Instance.GetAllLinkedAgencies(agencyNumber);
            ViewData["LinkedAgencies"] = new SelectList(agencies.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList(), "Value", "Text");

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(ClientViewModel Model) {
            if (Model.FirstName == null)
                return Json(new { Success = false, ErrorDescription = "El Nombre del cliente es requerido" });
            if (Model.LastName == null)
                return Json(new { Success = false, ErrorDescription = "Los Apellidos del cliente son requeridos" });
            if (Model.LinkedToAgencyId == null) {
                if (Model.Birthday == null)
                    return Json(new { Success = false, ErrorDescription = "La Fecha de nacimiento del cliente es requerida" });
                if (Model.Phone == null)
                    return Json(new { Success = false, ErrorDescription = "El Teléfono del cliente es requerido" });
            }

            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            string username = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);
            
            ClientDTO client = new ClientDTO();
            client.CreatedByUsername = username;
            client.LinkedToAgencyId = Model.LinkedToAgencyId;
            client.FirstName = Model.FirstName;
            client.LastName = Model.LastName;
            client.Birthday = Model.Birthday == null || Model.Birthday == string.Empty ? (DateTime?)null : DateTime.Parse(Model.Birthday, new System.Globalization.CultureInfo("es-ES"));
            client.Phone = Model.LinkedToAgencyId == null ? ApplicationHelper.Instance.FormatPhoneNumber(Model.Phone, true).Value : null;
            client.Email = Model.Email;
            client.Notes = Model.Notes;

            int id = await Client.Instance.AddClient(agencyNumber, client);

            return Json(new { Success = id != -1, ErrorDescription = id != -1 ? string.Empty : "Un ocurrió adicionando este cliente. Por favor cierre su sesión y abrala nuevamente.", Data = id });
        }

        public async Task<ActionResult> Edit(int ClientId, string ReturnUrl) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            ClientDTO client = await Client.Instance.GetClientById(agencyNumber, ClientId);
            
            ClientViewModel model = new ClientViewModel();
            model.Id = client.Id;
            model.CreatedByUserId = client.CreatedByUserId;
            model.CreatedOn = client.CreatedOn;
            model.ModifiedByUserId = client.ModifiedByUserId;
            model.ModifiedOn = client.ModifiedOn;
            model.FirstName = client.FirstName;
            model.LastName = client.LastName;
            model.Birthday = client.Birthday == null ? string.Empty : ((DateTime)client.Birthday).ToString("dd/MM/yyyy");

            KeyValuePair<bool, string> formattedPhoneNumber = ApplicationHelper.Instance.FormatPhoneNumber(client.Phone, false);
            if (formattedPhoneNumber.Key)
            {
                model.Phone = formattedPhoneNumber.Value;
                model.Notes = client.Notes;
            }
            else {
                model.Phone = string.Empty;
                model.Notes = client.Notes == null || client.Notes == string.Empty ? "Teléfono: " + formattedPhoneNumber.Value : client.Notes + "\rTeléfono: " + formattedPhoneNumber.Value;
            }
            model.Email = client.Email;
            model.LinkedToAgencyId = client.LinkedToAgencyId;
            model.LinkedAgencyName = client.LinkedAgencyName;
            model.Troubled = client.Troubled;

            List<LinkedAgencyDTO> agencies = await Client.Instance.GetAllLinkedAgencies(agencyNumber);
            ViewData["LinkedAgencies"] = new SelectList(agencies.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList(), "Value", "Text");

            ViewData["ReturnUrl"] = ReturnUrl;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ClientViewModel Model)
        {
            if (Model.FirstName == null)
                return Json(new { Success = false, ErrorDescription = "El Nombre del cliente es requerido" });
            if (Model.LastName == null)
                return Json(new { Success = false, ErrorDescription = "Los Apellidos del cliente son requeridos" });
            if (Model.LinkedToAgencyId == null) {
                if (Model.Birthday == null)
                    return Json(new { Success = false, ErrorDescription = "La Fecha de nacimiento del cliente es requerida" });
                if (Model.Phone == null)
                    return Json(new { Success = false, ErrorDescription = "El Teléfono del cliente es requerido" });
            }

            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            string username = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);
            
            ClientDTO client = new ClientDTO();
            client.Id = Model.Id;
            client.CreatedByUserId = Model.CreatedByUserId;
            client.CreatedOn = Model.CreatedOn;
            client.ModifiedByUserId = Model.ModifiedByUserId;
            client.ModifiedOn = Model.ModifiedOn;
            client.ModifiedByUsername = username;
            client.LinkedToAgencyId = Model.LinkedToAgencyId;
            client.FirstName = Model.FirstName;
            client.LastName = Model.LastName;
            client.Birthday = Model.Birthday == null || Model.Birthday == string.Empty ? (DateTime?)null : DateTime.Parse(Model.Birthday, new System.Globalization.CultureInfo("es-ES"));
            client.Phone = Model.LinkedToAgencyId == null ? ApplicationHelper.Instance.FormatPhoneNumber(Model.Phone, true).Value : null;
            client.Email = Model.Email;
            client.Notes = Model.Notes;

            bool updated = await Client.Instance.UpdateClient(agencyNumber, client);

            return Json(new { Success = updated, ErrorDescription = updated ? string.Empty : "Un ocurrió modificando este cliente. Por favor cierre su sesión y abrala nuevamente." });
        }

        [HttpPost]
        public async Task<ActionResult> MarkAsTroubled(int ClientId, bool Mark) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            ClientDTO client = await Client.Instance.GetClientById(agencyNumber, ClientId);

            client.ModifiedByUsername = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);
            client.Troubled = Mark;

            bool updated = await Client.Instance.UpdateClient(agencyNumber, client);

            return Json(new { Success = updated });
        }

        [HttpPost]
        public async Task<ActionResult> Remove(int ClientId) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            bool removed = await Client.Instance.RemoveClient(agencyNumber, ClientId);

            return Json(new { Success = removed });
        }

        #region Helpers
        
        #endregion
    }
}