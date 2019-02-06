using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppsManager.DL;
using Cars.DL;
using Cars.Reports.DataSets;
using Microsoft.Reporting.WinForms;

namespace Cars.Reports
{
    public class PaymentTypesReportModel : ReportModel
    {
        #region Constructor
        public PaymentTypesReportModel() {
            Name = "Tipos de pagos";
            Url = "paymenttypes";
        }
        #endregion

        #region Private Variables
        private Agency agency;
        private DateTime fromDate;
        private DateTime toDate;
        private int totalPaymentTypes;
        private string printedby;
        Dictionary<string, double> paymentTypes;
        #endregion

        #region Virtual Methods
        public override void Initialize(string AgencyNumber, string Format, string Parameters)
        {
            this.AgencyNumber = AgencyNumber;
            this.Format = Format;

            //Get Parameters: Parameters should appear in the following structure:
            //param1name=param1value;param2ame=param2value
            fromDate = DateTime.Today;
            toDate = DateTime.Today;
            string[] paramSet = Parameters.Split(';');
            foreach (string param in paramSet)
            {
                string[] paramDict = param.Split('=');
                if (paramDict[0].ToUpper() == "FROMDATE") fromDate = Convert.ToDateTime(paramDict[1]);
                if (paramDict[0].ToUpper() == "TODATE") toDate = Convert.ToDateTime(paramDict[1]);
                if (paramDict[0].ToUpper() == "PRINTEDBY") printedby = paramDict[1];
            }

            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            //Load Agency
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                agency = ageMan.GetByNumber(AgencyNumber);

                //Load Payment Methods
                PaymentMethodsManager payMan = new PaymentMethodsManager();
                paymentTypes = payMan.GetReportOfPaymentTypesPerPeriod(AgencyNumber, fromDate, toDate);
                totalPaymentTypes = paymentTypes.Count();
            }
        }
        protected override object Pdf(bool IsImplemented)
        {
            if (IsImplemented) return true;
            return GetReport("PDF");
        }
        protected override object Excel(bool IsImplemented)
        {
            if (IsImplemented) return true;
            return GetReport("Excel");
        }
        #endregion

        #region Helpers
        private ReportDataSource GetHeaderDataSource() {
            PaymentTypes dataSet = new PaymentTypes();
            dataSet.Tables["HeaderTable"].Rows.Add(new object[] {
                fromDate.ToString("dd/MM/yyyy"),
                toDate.ToString("dd/MM/yyyy"),
                totalPaymentTypes,
                agency.LogoFilename,
                printedby
            });

            ReportDataSource dataSource = new ReportDataSource("Header", dataSet.Tables["HeaderTable"]);
            return dataSource;
        }
        private ReportDataSource GetCarReservationsDataSource() {
            PaymentTypes dataSet = new PaymentTypes();
            foreach (KeyValuePair<string, double> kvp in paymentTypes)
            {
                dataSet.Tables["DetailsTable"].Rows.Add(new object[] {
                    kvp.Key,
                    kvp.Value
                });
            }

            ReportDataSource dataSource = new ReportDataSource("PaymentDetails", dataSet.Tables["DetailsTable"]);
            return dataSource;
        }
        private object GetReport(string Format) {
            //Initialize Report
            string rootPath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "bin\\Reports\\");
            //string rootPath = @"C:\Users\Administrator\Source\Workspaces\StardomProject\Solution\Cars.Reports\Reports\";
            LocalReport report = new LocalReport();
            report.EnableExternalImages = true;
            report.ReportPath = rootPath + "PaymentTypes.rdlc";
            report.DataSources.Add(GetHeaderDataSource());
            report.DataSources.Add(GetCarReservationsDataSource());
            report.Refresh();

            //Prepare for Render
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            //Render
            byte[] bytes = report.Render(Format, null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            //using (FileStream fs = new FileStream(@"c:\temp\output.pdf", FileMode.Create))
            //{
            //    fs.Write(bytes, 0, bytes.Length);
            //}

            //Return
            return bytes;
        }
        #endregion
    }
}
