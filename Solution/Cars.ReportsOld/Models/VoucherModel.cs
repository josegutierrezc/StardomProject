using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cars.DL;
using AppsManager.DL;
using System.IO;
using Microsoft.Reporting.WinForms;
using Cars.Reports.DataSets;

namespace Cars.Reports
{
    public class VoucherModel : ReportModel
    {
        #region Constructor
        public VoucherModel() {
            Name = "Voucher";
            Url = "voucher";
        }
        #endregion

        #region Private Variables
        private CarReservationExtension entity;
        private Agency agency;
        private string imageFilename;
        #endregion

        #region Virtual Methods
        public override void Initialize(string AgencyNumber, string Format, string Parameters)
        {
            this.AgencyNumber = AgencyNumber;
            this.Format = Format;

            //Get Parameters: Parameters should appear in the following structure:
            //param1name=param1value;param2ame=param2value
            long carReservationId = -1;
            string[] paramSet = Parameters.Split(';');
            foreach (string param in paramSet) {
                string[] paramDict = param.Split('=');
                if (paramDict[0].ToUpper() == "CARRESERVATIONID") {
                    carReservationId = Convert.ToInt64(paramDict[1]);
                    break;
                }
            }

            //Load Agency
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                AgenciesManager ageMan = new AgenciesManager(DB);
                agency = ageMan.GetByNumber(AgencyNumber);

                //Load Car Reservation
                CarReservationsManager carsMan = new CarReservationsManager();
                entity = carsMan.GetById(AgencyNumber, carReservationId);
                entity.ToDate = new DateTime(entity.ToDate.Year, entity.ToDate.Month, entity.ToDate.Day, entity.FromDate.Hour, entity.FromDate.Minute, entity.FromDate.Second);
                entity.Days = entity.ToDate.Subtract(entity.FromDate).Days;

                //Get TourOperator Logo Filename
                TourOperatorsManager tourMan = new TourOperatorsManager();
                imageFilename = tourMan.GetImageFilename(AgencyNumber, (int)entity.TourOperatorId);
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
        private ReportDataSource GetDataSource() {
            Voucher dataSet = new Voucher();
            dataSet.Tables[0].Rows.Add(new object[] {
                entity.ClientFirstname.ToUpper() + " " + entity.ClientLastname.ToUpper(),
                entity.TourOperatorName,
                entity.ReservationNumber,
                entity.FromDate.ToString("dd/MM/yyyy"),
                entity.ToDate.ToString("dd/MM/yyyy"),
                entity.Days,
                entity.FromDate.ToShortTimeString(),
                "Cuba",
                entity.FlightNumber,
                entity.CarCategoryName,
                entity.RentCarPlaceName,
                "LIBRE",
                "RENTA + SEGURO",
                agency.Disclaimer,
                imageFilename
            });

            ReportDataSource dataSource = new ReportDataSource("Voucher", dataSet.Tables[0]);
            return dataSource;
        }
        private object GetReport(string Format) {
            //Initialize Report
            string rootPath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "bin\\Reports\\");
            //string rootPath = @"C:\Users\Administrator\Source\Workspaces\StardomProject\Solution\Cars.Reports\Reports\";
            LocalReport report = new LocalReport();
            report.EnableExternalImages = true;
            report.ReportPath = rootPath + "Voucher.rdlc";
            report.DataSources.Add(GetDataSource());
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
