using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cars.DTO;
using Cars.REST.Client;
using BookingManager.Web.Models;
using BookingManager.Web.Helpers;

namespace BookingManager.Web.Controllers
{
    [Authorize]
    public class CarReportsController : Controller
    {
        public ActionResult Index()
        {
            ReportsViewModel model = new ReportsViewModel();
            model.Reports = new SelectList(GetAllReports(), "ReportId", "ReportName");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Get(int? ReportId) {
            ReportsViewModel model = new ReportsViewModel();
            ReportViewModel[] reports = GetAllReports();
            model.Reports = new SelectList(reports, "ReportId", "ReportName");
            model.Formats = new SelectList(GetReportFormats(), "Key", "Value");
            model.TourOperators = new SelectList(await GetAllTourOperators(), "Id", "Name");
            model.PaymentsStatuses = new SelectList(await GetAllPaymentStatuses(), "Id", "Name");

            model.SelectedReport = ReportId == null ? null : reports[(int)ReportId - 1];
            return PartialView("_ReportFilters", model);
        }

        [HttpPost]
        public ActionResult ValidateReportParameters(ReportsViewModel Model) {
            //Validating
            if (Model.SelectedReport.Format == null)
                return Json(new { Success = false, ErrorDescription = "No fué especificado ningún formato de exportación." });
            if (Model.SelectedReport.IsFromDateEnabled && Model.SelectedReport.FromDate == null)
                return Json(new { Success = false, ErrorDescription = "La fecha de comienzo no fué especificada." });
            if (Model.SelectedReport.IsToDateEnabled && Model.SelectedReport.ToDate == null)
                return Json(new { Success = false, ErrorDescription = "La fecha de finalización no fué especificada." });
            return Json(new { Success = true });
        }

        public async Task<ActionResult> DownloadReport(int ReportId, string Format, string FromDate, string ToDate, int? TourOperatorId, int? PaymentStatusId)
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            string printedBy = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UserFullnameTagNam);

            int tourOperatorId = TourOperatorId == null ? -1 : (int)TourOperatorId;
            int paymentStatusId = PaymentStatusId == null ? -1 : (int)PaymentStatusId;

            byte[] voucher = new byte[] { };
            if (ReportId == 1)
                voucher = await Client.Instance.GetAccountingReport(agencyNumber, Format, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), tourOperatorId, paymentStatusId, printedBy);
            else if (ReportId == 2)
                voucher = await Client.Instance.GetAccountingLimitedReport(agencyNumber, Format, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), tourOperatorId, paymentStatusId, printedBy);
            else if (ReportId == 3)
                voucher = await Client.Instance.GetPaymentTypesReport(agencyNumber, Format, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), printedBy);
            else if (ReportId == 4)
                voucher = await Client.Instance.GetSalesByAgentReport(agencyNumber, Format, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), printedBy);
            else
                voucher = await Client.Instance.GetReservationsListReport(agencyNumber, Format, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), printedBy);

            if (Format.ToLower() == "pdf") return File(voucher, System.Net.Mime.MediaTypeNames.Application.Pdf);
            return File(voucher, System.Net.Mime.MediaTypeNames.Application.Octet, "Reporte.xls");
        }

        #region Helpers
        private ReportViewModel[] GetAllReports() {
            ReportViewModel[] reports = new ReportViewModel[] {
                new ReportViewModel() {
                    ReportId = 1,
                    ReportName = "Contabilidad",
                    IsFromDateEnabled = true,
                    IsToDateEnabled = true,
                    IsTourOperatorEnabled = true,
                    IsPaymentStatusEnabled = true
                },
                new ReportViewModel() {
                    ReportId = 2,
                    ReportName = "Contabilidad Limitada",
                    IsFromDateEnabled = true,
                    IsToDateEnabled = true,
                    IsTourOperatorEnabled = true,
                    IsPaymentStatusEnabled = true
                },
                new ReportViewModel() {
                    ReportId = 3,
                    ReportName = "Resumen de pagos",
                    IsFromDateEnabled = true,
                    IsToDateEnabled = true,
                    IsTourOperatorEnabled = false,
                    IsPaymentStatusEnabled = false,
                },
               new ReportViewModel() {
                   ReportId = 4,
                   ReportName = "Venta por operador",
                   IsFromDateEnabled = true,
                   IsToDateEnabled = true,
                   IsTourOperatorEnabled = false,
                   IsPaymentStatusEnabled = false,
               },
               new ReportViewModel() {
                   ReportId = 5,
                   ReportName = "Listado de Reservas",
                   IsFromDateEnabled = true,
                   IsToDateEnabled = true,
                   IsTourOperatorEnabled = false,
                   IsPaymentStatusEnabled = false 
               }
            };
            return reports;
        }
        private List<KeyValuePair<string, string>> GetReportFormats() {
            List<KeyValuePair<string, string>> formats = new List<KeyValuePair<string, string>>();
            formats.Add(new KeyValuePair<string, string>("pdf", "PDF"));
            formats.Add(new KeyValuePair<string, string>("excel", "Excel"));
            return formats;
        }
        private async Task<List<TourOperatorDTO>> GetAllTourOperators() {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            return await Client.Instance.GetTourOperators(agencyNumber, true);
        }
        private async Task<List<PaymentStatusDTO>> GetAllPaymentStatuses()
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            return await Client.Instance.GetPaymentStatuses(agencyNumber);
        }
        #endregion
    }
}