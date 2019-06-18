using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Reports
{
    public class ReportModel
    {
        #region Constants
        public const string PdfFormat = "PDF";
        public const string HtmlFormat = "HTML";
        public const string ExcelFormat = "EXCEL";
        #endregion

        #region Variables
        protected Dictionary<string, object> Parameters;
        public string AgencyNumber;
        public string Format;
        public string Name;
        public string Url;
        #endregion

        #region Static Members
        public static ReportModel[] All = new ReportModel[] { new VoucherModel(), new PaymentReceiptModel(), new AccountingReportModel(), new AccountingLimitedReportModel(), new PaymentTypesReportModel(), new SalesByAgentReportModel(), new CarReservationsModel(), new CarReservationStatusReportModel() };
        #endregion

        #region Constructors
        public ReportModel() {
            Parameters = new Dictionary<string, object>();
        }
        #endregion

        #region Virtual Methods
        public virtual void Initialize(string AgencyNumber, string Format, string Parameters) { return; }
        protected virtual object Pdf(bool IsImplemented) { return false; }
        protected virtual object Html(bool IsImplemented) { return false; }
        protected virtual object Excel(bool IsImplemented) { return false; }
        #endregion

        #region Methods
        public object Generate() {
            if (Format.ToUpper() == PdfFormat) return Pdf(false);
            if (Format.ToUpper() == HtmlFormat) return Html(false);
            if (Format.ToUpper() == ExcelFormat) return Excel(false);
            return null;
        }
        #endregion
    }
}
