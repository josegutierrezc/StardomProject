using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cars.REST.Client;
using Cars.DTO;
using AppsManager.DTO;
using BookingManager.Web.Models;
using BookingManager.Web.Helpers;
using System.Threading.Tasks;
using System.IO;
using System.Web.Routing;

namespace BookingManager.Web.Controllers
{
    [Authorize]
    public class CarReservationsController : Controller
    {
        public async Task<ActionResult> Index()
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            CarReservationsLogModel model = new CarReservationsLogModel();
            model.PageSize = 50;
            model.PageNumber = 1;
            model.SearchFor = string.Empty;
            model.Filter = string.Empty;
            KeyValuePair<int, List<CarReservationDTO>> result = await Client.Instance.GetCarReservations(agencyNumber, "active", model.PageSize, model.PageNumber, model.SearchFor, model.Filter);
            model.ActiveReservations = result.Value;
            model.TotalPages = result.Key % model.PageSize == 0 ? result.Key / model.PageSize : result.Key / model.PageSize + 1;

            model.HistoryPageSize = 50;
            model.HistoryPageNumber = 1;
            model.HistorySearchFor = string.Empty;
            model.HistoryFilter = string.Empty;
            KeyValuePair<int, List<CarReservationDTO>> historyresult = await Client.Instance.GetCarReservations(agencyNumber, "history", model.HistoryPageSize, model.HistoryPageNumber, model.HistorySearchFor, model.HistoryFilter);
            model.HistoryReservations = historyresult.Value;
            model.HistoryTotalPages = historyresult.Key % model.HistoryPageSize == 0 ? historyresult.Key / model.HistoryPageSize : historyresult.Key / model.HistoryPageSize + 1;

            return View(model);
        }

        public async Task<ActionResult> Actives(string PageSize, string PageNumber, string SearchFor, string Filter) {
            CarReservationsLogModel model = new CarReservationsLogModel();
            model.PageSize = Convert.ToInt32(PageSize);
            model.PageNumber = Convert.ToInt32(PageNumber);
            model.SearchFor = SearchFor;
            model.Filter = Filter;

            KeyValuePair<int, List<CarReservationDTO>> result = await Client.Instance.GetCarReservations(ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName), "active", model.PageSize, model.PageNumber, model.SearchFor, model.Filter);
            model.ActiveReservations = result.Value;
            model.TotalPages = result.Key % model.PageSize == 0 ? result.Key / model.PageSize : result.Key / model.PageSize + 1;

            return PartialView("_ActiveReservations", model);
        }

        public async Task<ActionResult> History(string PageSize, string PageNumber, string SearchFor, string Filter)
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            CarReservationsLogModel model = new CarReservationsLogModel();
            model.HistoryPageSize = Convert.ToInt32(PageSize);
            model.HistoryPageNumber = Convert.ToInt32(PageNumber);
            model.HistorySearchFor = SearchFor;
            model.HistoryFilter = Filter;

            KeyValuePair<int, List<CarReservationDTO>> result = await Client.Instance.GetCarReservations(agencyNumber, "history", model.HistoryPageSize, model.HistoryPageNumber, model.HistorySearchFor, model.HistoryFilter);
            model.HistoryReservations = result.Value;
            model.HistoryTotalPages = result.Key % model.HistoryPageSize == 0 ? result.Key / model.HistoryPageSize : result.Key / model.HistoryPageSize + 1;

            return PartialView("_HistoryReservations", model);
        }

        public async Task<ActionResult> Add(int ClientId)
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            ClientDTO client = await Client.Instance.GetClientById(agencyNumber, ClientId);

            CarReservationViewModel model = new CarReservationViewModel();
            model.AgencyNumber = agencyNumber;
            model.Id = -1;
            model.ClientId = client.Id;
            model.ClientFullname = (client.FirstName.Trim().ToUpper() + " " + client.LastName.Trim().ToUpper()).Trim();
            model.ClientPhone = client.Phone == null ? string.Empty : client.Phone.Trim();
            model.ClientEmail = client.Email == null ? string.Empty : client.Email.Trim();
            model.TroubledClient = client.Troubled;
            model.CarProviderId = -1;
            model.CarCategoryId = -1;
            model.TourOperatorId = -1;
            model.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
            model.FromHour = DateTime.Now.ToShortTimeString();
            model.ToDate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            model.ProvinceId = -1;
            model.RentCarPlaceId = -1;
            model.Note = string.Empty;
            model.Days = 1;
            model.HasInsurance = true;
            model.InsuranceCost = 0;
            model.CostPrice = 0;
            model.SalePrice = 0;
            model.TotalSalePrice = 0;
            model.Discount = 0;
            model.TotalPaid = 0;
            model.PriceConfigurationFound = false;
            model.PaymentConcepts = (await Client.Instance.GetPaymentConcepts(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();
            model.PaymentMethods = (await Client.Instance.GetPaymentMethods(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            List<CarProviderDTO> providers = await Client.Instance.GetCarProviders(model.AgencyNumber);
            ViewData["CarProviders"] = new SelectList(providers.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList(), "Value", "Text");

            ViewData["CarCategories"] = (await Client.Instance.GetCarCategories(model.AgencyNumber, model.CarProviderId == null ? providers[0].Id : (int)model.CarProviderId)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            ViewData["TourOperators"] = (await Client.Instance.GetTourOperators(model.AgencyNumber, model.CarProviderId == null ? providers[0].Id : (int)model.CarProviderId)).Where(e => e.IsActive).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            List<ProvinceDTO> provinces = await Client.Instance.GetProvinces(model.AgencyNumber);
            ViewData["Provinces"] = new SelectList(provinces.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList(), "Value", "Text");

            ViewData["RentCarPlaces"] = (await Client.Instance.GetActiveRentCarPlaces(model.AgencyNumber, model.ProvinceId == null ? provinces[0].Id : (int)model.ProvinceId)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            //Get Payments
            model.Payments = new List<CarReservationPaymentViewModel>();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(CarReservationViewModel Model) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            //Validate
            if (Model.CarProviderId == null | Model.CarProviderId == -1)
                return Json(new { Success = false, ErrorDescription = "Por favor defina al Proveedor de Autos." });
            if (Model.ProvinceId == null | Model.ProvinceId == -1)
                return Json(new { Success = false, ErrorDescription = "Por favor defina la Provincia." });

            //Get Price Configuration
            var ci = new System.Globalization.CultureInfo("es-ES");
            DateTime fromDate = DateTime.Parse(Model.FromDate, ci);
            DateTime toDate = DateTime.Parse(Model.ToDate, ci);
            var priceConfiguration = await Client.Instance.GetPriceConfiguration(agencyNumber, (int)Model.CarCategoryId, (int)Model.TourOperatorId, fromDate.ToShortDateString(), toDate.ToShortDateString());

            //Create DTO
            CarReservationDTO dto = new CarReservationDTO();

            //Update Data
            dto.PaymentStatusId = 1;
            dto.CarCategoryId = Model.CarCategoryId;
            dto.CarProviderId = Model.CarProviderId;
            dto.ClientId = Model.ClientId;
            dto.CostPrice = priceConfiguration == null ? 0 : priceConfiguration.CostPrice;
            dto.SalePrice = priceConfiguration == null ? 0 : priceConfiguration.SalePrice;
            dto.CreatedByUser = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);
            dto.Discount = Model.Discount;
            dto.FlightNumber = Model.FlightNumber;
            dto.FromDate = fromDate;
            dto.ToDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, fromDate.Hour, fromDate.Minute, fromDate.Second);
            dto.HasInsurance = false;
            dto.InsuranceCost = 0;
            dto.Note = Model.Note;
            dto.ProvinceId = Model.ProvinceId;
            dto.RentCarPlaceId = Model.RentCarPlaceId;
            dto.ReservationNumber = Model.ReservationNumber;
            dto.TourOperatorId = Model.TourOperatorId;

            //Add
            int reservationId = await Client.Instance.AddCarReservation(agencyNumber, dto);

            //Check
            if (reservationId == -1)
                return Json(new { Success = false, ErrorDescription = "El servidor respondio con un estado fallido. Por favor cierre su sesión y ábrala nuevamente." });

            if (Model.SaveAndStay)
                return Json(new { Success = true, ErrorDescription = string.Empty, CarReservationId = reservationId });
            return Json(new { Success = true, ErrorDescription = string.Empty, CarReservationId = -1 });
        }

        public async Task<ActionResult> Edit(long Id) {
            CarReservationViewModel model = new CarReservationViewModel();
            CarReservationDTO reservation = await Client.Instance.GetCarReservationById(ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName), Id);

            model.AgencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            model.Id = reservation.Id;
            model.PaymentStatusId = reservation.PaymentStatusId;
            model.CreatedByUser = reservation.CreatedByUser;
            model.CreatedOn = reservation.CreatedOn;
            model.ModifiedByUser = reservation.ModifiedByUser;
            model.ModifiedOn = reservation.ModifiedOn;
            model.CancelledBy = reservation.CancelledBy;
            model.CancelledByUser = reservation.CancelledByUser;
            model.CancelledOn = reservation.CancelledOn;
            model.IsCancelled = reservation.CancelledOn != null;
            model.ReservationNumber = reservation.ReservationNumber == null ? string.Empty : reservation.ReservationNumber.Trim();
            model.FlightNumber = reservation.FlightNumber == null ? string.Empty : reservation.FlightNumber.Trim();
            model.ClientId = reservation.ClientId;
            model.ClientFullname = (reservation.ClientFirstname.Trim().ToUpper() + " " + reservation.ClientLastname.Trim().ToUpper()).Trim();
            model.ClientPhone = reservation.ClientPhone == null ? string.Empty : reservation.ClientPhone.Trim();
            model.ClientEmail = reservation.ClientEmail == null ? string.Empty : reservation.ClientEmail.Trim();
            model.TroubledClient = reservation.TroubledClient;
            model.CarProviderId = reservation.CarProviderId;
            model.CarCategoryId = reservation.CarCategoryId;
            model.TourOperatorId = reservation.TourOperatorId;
            model.FromDate = reservation.FromDate.ToString("dd/MM/yyyy");
            model.FromHour = reservation.FromDate.ToShortTimeString();
            model.ToDate = reservation.ToDate.ToString("dd/MM/yyyy");
            model.ProvinceId = reservation.ProvinceId;
            model.RentCarPlaceId = reservation.RentCarPlaceId;
            model.Note = reservation.Note;
            model.Days = reservation.ToDate.Subtract(reservation.FromDate).Days;
            model.HasInsurance = reservation.HasInsurance;
            model.InsuranceCost = reservation.InsuranceCost;
            model.CostPrice = reservation.CostPrice;
            model.SalePrice = reservation.SalePrice;
            model.TotalSalePrice = reservation.SalePrice * model.Days;
            model.Discount = reservation.Discount;
            model.TotalPaid = await GetTotalPaid(model.AgencyNumber, model.Id, await Client.Instance.GetCarReservationPayments(model.AgencyNumber, model.Id));
            model.PriceConfigurationFound = true;
            model.LinkedAgencyName = reservation.LinkedAgencyName;
            model.LinkedAgencyPhone = reservation.LinkedAgencyPhone == null ? string.Empty : ApplicationHelper.Instance.FormatPhoneNumber(reservation.LinkedAgencyPhone, false).Value;
            model.LinkedAgencyEmail = reservation.LinkedAgencyEmail == null ? string.Empty : reservation.LinkedAgencyEmail;
            model.PaymentConcepts = (await Client.Instance.GetPaymentConcepts(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();
            model.PaymentMethods = (await Client.Instance.GetPaymentMethods(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            List<CarProviderDTO> providers = await Client.Instance.GetCarProviders(model.AgencyNumber);
            ViewData["CarProviders"] = providers.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            ViewData["CarCategories"] = (await Client.Instance.GetCarCategories(model.AgencyNumber, model.CarProviderId == null ? providers[0].Id : (int)model.CarProviderId)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            ViewData["TourOperators"] = (await Client.Instance.GetTourOperators(model.AgencyNumber, model.CarProviderId == null ? providers[0].Id : (int)model.CarProviderId)).Where(e => e.IsActive).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            List<ProvinceDTO> provinces = await Client.Instance.GetProvinces(model.AgencyNumber);
            ViewData["Provinces"] = provinces.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            ViewData["RentCarPlaces"] = (await Client.Instance.GetActiveRentCarPlaces(model.AgencyNumber, model.ProvinceId == null ? provinces[0].Id : (int)model.ProvinceId)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            //Get Payments
            List<CarReservationPaymentDTO> paymentsdto = await Client.Instance.GetCarReservationPayments(model.AgencyNumber, model.Id);
            model.TotalPaid = await GetTotalPaid(model.AgencyNumber, model.Id, paymentsdto);
            model.Payments = GetCarReservationPaymentsViewModel(model.AgencyNumber, model.Id, paymentsdto);
            model.FinalPrice = (double)model.TotalSalePrice - (double)model.Discount * model.Days;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CarReservationViewModel Model) {
            //Get Reservation from DB
            CarReservationDTO reservation = await Client.Instance.GetCarReservationById(Model.AgencyNumber, Model.Id);

            //Get Price Configuration
            DateTime fromDate = DateTime.Parse(Model.FromDate + " " + Model.FromHour, new System.Globalization.CultureInfo("es-ES"));
            DateTime toDate = DateTime.Parse(Model.ToDate + " " + Model.FromHour, new System.Globalization.CultureInfo("es-ES"));
            
            //Transfer Data
            reservation.ModifiedByUser = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);
            reservation.ClientId = Model.ClientId;
            reservation.ReservationNumber = Model.ReservationNumber;
            reservation.FlightNumber = Model.FlightNumber;
            reservation.CarProviderId = Model.CarProviderId;
            reservation.CarCategoryId = Model.CarCategoryId;
            reservation.TourOperatorId = Model.TourOperatorId;
            reservation.FromDate = fromDate;
            reservation.ToDate = toDate;
            reservation.ProvinceId = Model.ProvinceId;
            reservation.RentCarPlaceId = Model.RentCarPlaceId;
            reservation.HasInsurance = Model.HasInsurance;
            reservation.InsuranceCost = Model.InsuranceCost;
            reservation.Discount = Model.Discount;
            reservation.Note = Model.Note == null ? string.Empty : Model.Note;
            reservation.CostPrice = Model.CostPrice;
            reservation.SalePrice = Model.SalePrice;

            KeyValuePair<bool, string> updated = await Client.Instance.UpdateCarReservation(Model.AgencyNumber, reservation);

            return Model.SaveAndStay ? Json(new { Success = updated.Key, ErrorDescription = updated.Value, CarReservationId = Model.Id }) : Json(new { Success = updated.Key, ErrorDescription = updated.Value, CarReservationId = -1 });
        }

        [HttpPost]
        public async Task<ActionResult> Cancel(long CarReservationId) {
            //Get AgencyNumber
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            //Get Username
            string username = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);

            //Get Reservation from DB
            CarReservationDTO reservation = await Client.Instance.GetCarReservationById(agencyNumber, CarReservationId);

            //Update Reservation
            reservation.ModifiedByUser = username;
            reservation.CancelledByUser = username;
            reservation.CancelledOn = DateTime.Now;

            //Update
            KeyValuePair<bool, string> updated = await Client.Instance.UpdateCarReservation(agencyNumber, reservation);

            return Json(new { Success = updated.Key, ErrorDescription = updated.Value });

        }

        [HttpPost]
        public async Task<ActionResult> Recover(long CarReservationId)
        {
            //Get AgencyNumber
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            //Get Username
            string username = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);

            //Get Reservation from DB
            CarReservationDTO reservation = await Client.Instance.GetCarReservationById(agencyNumber, CarReservationId);

            //Update Reservation
            reservation.ModifiedByUser = username;
            reservation.CancelledByUser = string.Empty;
            reservation.CancelledOn = null;

            //Update
            KeyValuePair<bool, string> updated = await Client.Instance.UpdateCarReservation(agencyNumber, reservation);

            return Json(new { Success = updated.Key, updated.Value });
        }

        public async Task<ActionResult> CarCategories(string AgencyNumber, int CarProviderId) {
            if (CarProviderId == -1)
                return Json(new { Data = new List<CarCategoryDTO>() }, JsonRequestBehavior.AllowGet);

            var categories = await Client.Instance.GetCarCategories(AgencyNumber, CarProviderId);
            return Json(new { Data = categories }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> TourOperators(string AgencyNumber, int CarProviderId)
        {
            if (CarProviderId == -1)
                return Json(new { Data = new List<TourOperatorDTO>() }, JsonRequestBehavior.AllowGet);

            var operators = (await Client.Instance.GetTourOperators(AgencyNumber, CarProviderId)).Where(e => e.IsActive);
            return Json(new { Data = operators }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RentCarPlaces(string AgencyNumber, int ProvinceId)
        {
            if (ProvinceId == -1)
                return Json(new { Data = new List<RentCarPlaceDTO>() }, JsonRequestBehavior.AllowGet);
            var operators = await Client.Instance.GetActiveRentCarPlaces(AgencyNumber, ProvinceId);
            return Json(new { Data = operators }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdatePrices(string AgencyNumber, int CarCategoryId, int TourOperatorId, string FromDate, string ToDate, int Discount, double CurrentCostPrice, double CurrentSalePrice) {
            var ci = new System.Globalization.CultureInfo("es-ES");
            DateTime fromDate = DateTime.Parse(FromDate, ci);
            DateTime toDate = DateTime.Parse(ToDate, ci);
            var priceConfiguration = await Client.Instance.GetPriceConfiguration(AgencyNumber, CarCategoryId, TourOperatorId, fromDate.ToShortDateString(), toDate.ToShortDateString());

            CarReservationViewModel model = new CarReservationViewModel();
            model.Days = toDate.Subtract(fromDate).Days;
            model.Discount = Discount;
            model.PriceConfigurationFound = priceConfiguration != null;
            if (priceConfiguration != null)
            {
                model.CostPrice = priceConfiguration.CostPrice;
                model.SalePrice = priceConfiguration.SalePrice;
                model.TotalSalePrice = priceConfiguration.SalePrice * model.Days;
            }
            else
            {
                model.CostPrice = CurrentCostPrice;
                model.SalePrice = CurrentSalePrice;
                model.TotalSalePrice = CurrentSalePrice * model.Days;
            }
            model.FinalPrice = (double)model.TotalSalePrice - (double)model.Discount * model.Days;


            return PartialView("_PriceConfiguration", model);
        }

        [HttpPost]
        public async Task<ActionResult> AddPayment(string AgencyNumber, int CarReservationId, int PaymentConceptId, int PaymentMethodId, double Amount, bool IsReimbursement) {
            CarReservationPaymentDTO paymentdto = new CarReservationPaymentDTO();
            paymentdto.CreatedByUser = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);
            paymentdto.ReservationId = CarReservationId;
            paymentdto.ConceptId = PaymentConceptId;
            paymentdto.MethodId = PaymentMethodId;
            paymentdto.Amount = Amount;
            paymentdto.IsReimbursement = IsReimbursement;

            bool added = await Client.Instance.AddCarReservationPayment(AgencyNumber, CarReservationId, paymentdto);

            CarReservationViewModel model = new CarReservationViewModel();
            model.AgencyNumber = AgencyNumber;
            model.Id = CarReservationId;
            model.NewPayment = new CarReservationPaymentViewModel();
            model.NewPayment.IsReimbursement = false;
            model.PaymentConcepts = (await Client.Instance.GetPaymentConcepts(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();
            model.PaymentMethods = (await Client.Instance.GetPaymentMethods(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            //Get Payments
            List<CarReservationPaymentDTO> paymentsdto = await Client.Instance.GetCarReservationPayments(model.AgencyNumber, model.Id);
            model.TotalPaid = await GetTotalPaid(model.AgencyNumber, model.Id, paymentsdto);
            model.Payments = GetCarReservationPaymentsViewModel(model.AgencyNumber, model.Id, paymentsdto);

            return Json(new { Success = added, Data = RenderRazorViewToString("_CarReservationPayments", model) });
        }

        [HttpPost]
        public async Task<ActionResult> RemovePayment(string AgencyNumber, int CarReservationId, int PaymentId)
        {
            bool removed = await Client.Instance.RemoveCarReservationPayment(AgencyNumber, CarReservationId, PaymentId);

            CarReservationViewModel model = new CarReservationViewModel();
            model.AgencyNumber = AgencyNumber;
            model.Id = CarReservationId;
            model.PaymentConcepts = (await Client.Instance.GetPaymentConcepts(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();
            model.PaymentMethods = (await Client.Instance.GetPaymentMethods(model.AgencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList();

            //Get Payments
            List<CarReservationPaymentDTO> paymentsdto = await Client.Instance.GetCarReservationPayments(model.AgencyNumber, model.Id);
            model.TotalPaid = await GetTotalPaid(model.AgencyNumber, model.Id, paymentsdto);
            model.Payments = GetCarReservationPaymentsViewModel(model.AgencyNumber, model.Id, paymentsdto);

            return Json(new { Success = removed, Data = RenderRazorViewToString("_CarReservationPayments", model) });
        }

        public async Task<ActionResult> AssignClient(long CarReservationId, int ClientId) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            CarReservationDTO dto = await Client.Instance.GetCarReservationById(agencyNumber, CarReservationId);
            dto.ClientId = ClientId;

            KeyValuePair<bool, string> updated = await Client.Instance.UpdateCarReservation(agencyNumber, dto);

            return new RedirectResult(Url.Action("Edit") + "?Id=" + CarReservationId);
        }

        public  async Task<ActionResult> DownloadVoucher(long CarReservationId, string Format) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            byte[] voucher = await Client.Instance.GetVoucherReport(agencyNumber, Format, CarReservationId);
            return File(voucher, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        public async Task<ActionResult> DownloadPaymentReceipt(long CarReservationId, string Format)
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            string printedBy = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UserFullnameTagNam);
            byte[] receipt = await Client.Instance.GetPaymentReceiptReport(agencyNumber, Format, CarReservationId, printedBy);
            return File(receipt, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }
        #region Helpers
        private async Task<double> GetTotalPaid(string AgencyNumber, long CarReservationId, List<CarReservationPaymentDTO> Payments) {
            Dictionary<int, PaymentConceptDTO> paymentConcepts = new Dictionary<int, PaymentConceptDTO>();
            foreach (PaymentConceptDTO c in await Client.Instance.GetPaymentConcepts(AgencyNumber))
                paymentConcepts.Add(c.Id, c);
            
            double total = 0;
            foreach (CarReservationPaymentDTO payment in Payments) {
                if (paymentConcepts[payment.ConceptId].AffectFinalPrice)
                {
                    if (payment.IsReimbursement)
                        total -= payment.Amount;
                    else
                        total += payment.Amount;
                }
            }

            return total;
        }
        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        private List<CarReservationPaymentViewModel> GetCarReservationPaymentsViewModel(string AgencyNumber, long CarReservationId, List<CarReservationPaymentDTO> Payments) {
            List<CarReservationPaymentViewModel> payments = new List<CarReservationPaymentViewModel>();
            foreach (CarReservationPaymentDTO payment in Payments)
            {
                CarReservationPaymentViewModel paymentModel = new CarReservationPaymentViewModel();
                paymentModel.Id = payment.Id;
                paymentModel.AgencyNumber = AgencyNumber;
                paymentModel.CarReservationId = CarReservationId;
                paymentModel.CreatedByUser = payment.CreatedByUser;
                paymentModel.CreatedOn = payment.CreatedOn.ToString("dd MMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpper();
                paymentModel.CreatedAt = payment.CreatedOn.ToString("hh:mm tt");
                paymentModel.ConceptId = payment.ConceptId;
                paymentModel.ConceptName = payment.ConceptName;
                paymentModel.MethodId = payment.MethodId;
                paymentModel.MethodName = payment.MethodName;
                paymentModel.Amount = payment.Amount;
                paymentModel.IsReimbursement = payment.IsReimbursement;
                payments.Add(paymentModel);
            }
            return payments;
        }
        #endregion
    }
}