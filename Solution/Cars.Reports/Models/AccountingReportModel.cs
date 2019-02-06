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
    public class AccountingReportModel : ReportModel
    {
        #region Constructor
        public AccountingReportModel() {
            Name = "Contabilidad";
            Url = "accounting";
        }
        #endregion

        #region Private Variables
        private Agency agency;
        private DateTime fromDate;
        private DateTime toDate;
        private int touroperatorid;
        private string touroperatorname;
        private int paymentstatusid;
        private string paymentstatusname;
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
            paymentstatusid = -1;
            touroperatorid = -1;
            string[] paramSet = Parameters.Split(';');
            foreach (string param in paramSet)
            {
                string[] paramDict = param.Split('=');
                if (paramDict[0].ToUpper() == "FROMDATE") fromDate = Convert.ToDateTime(paramDict[1]);
                if (paramDict[0].ToUpper() == "TODATE") toDate = Convert.ToDateTime(paramDict[1]);
                if (paramDict[0].ToUpper() == "TOUROPERATORID" & paramDict[1] != "-1") touroperatorid = Convert.ToInt32(paramDict[1]);
                if (paramDict[0].ToUpper() == "PAYMENTSTATUSID" & paramDict[1] != "-1") paymentstatusid = Convert.ToInt32(paramDict[1]);
                if (paramDict[0].ToUpper() == "PRINTEDBY") printedby = paramDict[1];
            }

            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            //Load Agency
            AgenciesManager ageMan = new AgenciesManager();
            agency = ageMan.GetByNumber(AgencyNumber);

            //Load AccountingReport
            CarReservationsManager carsMan = new CarReservationsManager();
            KeyValuePair<int, List<CarReservationExtension>> result = carsMan.GetAccountingReport(AgencyNumber, fromDate, toDate, touroperatorid, paymentstatusid);
            totalReservations = result.Key;
            reservations = result.Value;

            //Create a Tip Dictionary
            CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
            tipDictionary = new Dictionary<long, double>();
            foreach (CarReservationExtension r in reservations) {
                double totaltip = 0;
                foreach (PaymentExtension p in payMan.Get(AgencyNumber, r.Id))
                    if (p.ConceptId == 2) { totaltip += p.Amount; } 
                tipDictionary.Add(r.Id, totaltip);
            }

            //Define TourOperator Name if apply
            touroperatorname = "Todos incluidos";
            if (touroperatorid != -1) {
                TourOperatorsManager tourMan = new TourOperatorsManager();
                touroperatorname = tourMan.GetById(AgencyNumber, touroperatorid).Name;
            }

            //Define PaymentStatus Name
            paymentstatusname = paymentstatusid == -1 ? CarReservationsManager.PaymentStatusAllName + " incluidos" : CarReservationsManager.PaymentStatusNames[paymentstatusid];
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
            Accounting dataSet = new Accounting();
            dataSet.Tables["HeaderTable"].Rows.Add(new object[] {
                fromDate.ToString("dd/MM/yyyy"),
                toDate.ToString("dd/MM/yyyy"),
                totalReservations,
                touroperatorname,
                paymentstatusname,
                agency.LogoFilename,
                printedby
            });

            ReportDataSource dataSource = new ReportDataSource("Header", dataSet.Tables["HeaderTable"]);
            return dataSource;
        }
        private ReportDataSource GetCarReservationsDataSource() {
            Accounting dataSet = new Accounting();
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
        private object GetReport(string Format) {
            //Initialize Report
            string rootPath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "bin\\Reports\\");
            //string rootPath = @"C:\Users\Administrator\Source\Workspaces\StardomProject\Solution\Cars.Reports\Reports\";
            LocalReport report = new LocalReport();
            report.EnableExternalImages = true;
            report.ReportPath = rootPath + "Accounting.rdlc";
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
