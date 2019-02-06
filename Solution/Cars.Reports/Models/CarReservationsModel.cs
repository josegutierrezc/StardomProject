using AppsManager.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cars.DL;
using System.IO;
using Microsoft.Reporting.WinForms;
using Cars.Reports.DataSets;

namespace Cars.Reports
{
    public class CarReservationsModel : ReportModel
    {
        #region Constructor
        public CarReservationsModel()
        {
            Name = "Listado de Reservas";
            Url = "carreservations";
        }
        #endregion

        #region Private Variables
        private Agency agency;
        private DateTime fromDate;
        private DateTime toDate;
        private string printedby;
        private List<CarReservationExtension> reservations;
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
            AgenciesManager ageMan = new AgenciesManager();
            agency = ageMan.GetByNumber(AgencyNumber);

            //Get Reservations
            CarReservationsManager carMan = new CarReservationsManager();
            reservations = carMan.GetCarReservationsReport(AgencyNumber, fromDate, toDate);
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
            CarReservations dataSet = new CarReservations();
            dataSet.Tables["HeaderTable"].Rows.Add(new object[] {
                fromDate.ToString("dd/MM/yyyy"),
                toDate.ToString("dd/MM/yyyy"),
                reservations.Count(),
                agency.LogoFilename,
                printedby
            });

            ReportDataSource dataSource = new ReportDataSource("Header", dataSet.Tables["HeaderTable"]);
            return dataSource;
        }
        private ReportDataSource GetCarReservationsDataSource()
        {
            CarReservations dataSet = new CarReservations();
            foreach (CarReservationExtension cr in reservations)
            {
                cr.ToDate = new DateTime(cr.ToDate.Year, cr.ToDate.Month, cr.ToDate.Day, cr.FromDate.Hour, cr.FromDate.Minute, cr.FromDate.Second);
                cr.Days = cr.ToDate.Subtract(cr.FromDate).Days;
                dataSet.Tables["CarReservationTable"].Rows.Add(new object[] {
                    cr.ClientFirstname + " " + cr.ClientLastname,
                    cr.ReservationNumber,
                    cr.TourOperatorName,
                    cr.CarCategoryName,
                    cr.FromDate.ToString("dd/MM/yyyy"),
                    cr.Days,
                    cr.RentCarPlaceName,
                    cr.FlightNumber,
                    cr.FromDate.ToString("hh:mm tt"),
                    cr.Note
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
            report.ReportPath = rootPath + "CarReservations.rdlc";
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
