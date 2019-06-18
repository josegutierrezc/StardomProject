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
    public class CarReservationStatusReportModel : ReportModel
    {
        #region Constructor
        public CarReservationStatusReportModel()
        {
            Name = "Estado de reserva de auto";
            Url = "carreservationstatus";
        }
        #endregion

        #region Private Variables
        private Agency agency;
        private DateTime fromDate;
        private DateTime toDate;
        private string statusname;
        private int totalReservations;
        private string printedby;
        List<CarReservationExtension> reservations;
        Dictionary<long, double> tipDictionary;
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
            statusname = string.Empty;
            string[] paramSet = Parameters.Split(';');
            foreach (string param in paramSet)
            {
                string[] paramDict = param.Split('=');
                if (paramDict[0].ToUpper() == "FROMDATE") fromDate = Convert.ToDateTime(paramDict[1]);
                if (paramDict[0].ToUpper() == "TODATE") toDate = Convert.ToDateTime(paramDict[1]);
                if (paramDict[0].ToUpper() == "STATUS") statusname = paramDict[1];
                if (paramDict[0].ToUpper() == "PRINTEDBY") printedby = paramDict[1];
            }

            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            //Load Agency
            using (var DB = AppsManagerModel.ConnectToSqlServer())
            {
                AgenciesManager ageMan = new AgenciesManager(DB);
                agency = ageMan.GetByNumber(AgencyNumber);

                //Load AccountingReport
                CarReservationsManager carsMan = new CarReservationsManager();
                KeyValuePair<int, List<CarReservationExtension>> result = carsMan.GetCarReservationStatusReport(AgencyNumber, fromDate, toDate, statusname);
                totalReservations = result.Key;
                reservations = result.Value;

                //Create a Tip Dictionary
                CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
                tipDictionary = new Dictionary<long, double>();
                foreach (CarReservationExtension r in reservations)
                {
                    double totaltip = 0;
                    foreach (PaymentExtension p in payMan.Get(AgencyNumber, r.Id))
                        if (p.ConceptId == 2) { totaltip += p.Amount; }
                    tipDictionary.Add(r.Id, totaltip);
                }
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
        private ReportDataSource GetHeaderDataSource()
        {
            CarReservationStatus dataSet = new CarReservationStatus();
            dataSet.Tables["HeaderTable"].Rows.Add(new object[] {
                fromDate.ToString("dd/MM/yyyy"),
                toDate.ToString("dd/MM/yyyy"),
                0,
                statusname,
                agency.LogoFilename,
                printedby
            });

            ReportDataSource dataSource = new ReportDataSource("Header", dataSet.Tables["HeaderTable"]);
            return dataSource;
        }
        private ReportDataSource GetCarReservationsDataSource()
        {
            CarReservationStatus dataSet = new CarReservationStatus();
            foreach (CarReservationExtension res in reservations)
            {
                string paid = res.PaymentStatusId == CarReservationsManager.PaymentStatusUnpaid ? "No" : res.PaymentStatusId == CarReservationsManager.PaymentStatusPartiallyPaid ? "Parte" : res.PaymentStatusId == CarReservationsManager.PaymentStatusPaid ? "Si" : "Reemb";
                DateTime resToDate = new DateTime(res.ToDate.Year, res.ToDate.Month, res.ToDate.Day, res.FromDate.Hour, res.FromDate.Millisecond, res.FromDate.Second);
                int days = resToDate.Subtract(res.FromDate).Days;
                double totalCostPrice = (double)res.CostPrice * days;
                double totalSalePrice = (double)res.SalePrice * days - (double)res.Discount * days;
                dataSet.Tables["CarReservationTable"].Rows.Add(new object[] {
                    paid,
                    res.ClientFirstname + " " + res.ClientLastname,
                    res.ReservationNumber,
                    res.FromDate.ToString("dd/MM/yyyy"),
                    res.CarCategoryName,
                    days,
                    res.CostPrice,
                    totalCostPrice,
                    res.SalePrice,
                    totalSalePrice,
                    tipDictionary[res.Id],
                    totalSalePrice - totalCostPrice,
                    res.Note
                });
            }

            ReportDataSource dataSource = new ReportDataSource("CarReservations", dataSet.Tables["CarReservationTable"]);
            return dataSource;
        }
        private object GetReport(string Format)
        {
            //Initialize Report
            string rootPath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "bin\\Reports\\");
            //string rootPath = @"C:\Users\Administrator\Source\Workspaces\StardomProject\Solution\Cars.Reports\Reports\";
            LocalReport report = new LocalReport();
            report.EnableExternalImages = true;
            report.ReportPath = rootPath + "CarReservationStatus.rdlc";
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

            using (FileStream fs = new FileStream(@"c:\temp\output.pdf", FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }

            return null;

            //Return
            //return bytes;
        }
        #endregion
    }
}
