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
    public class PaymentReceiptModel : ReportModel
    {
        #region Constructor
        public PaymentReceiptModel()
        {
            Name = "PaymentReceipt";
            Url = "paymentreceipt";
        }
        #endregion

        #region Private Variables
        private CarReservationExtension entity;
        private List<PaymentExtension> payments;
        private Agency agency;
        private string printedby;
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
            foreach (string param in paramSet)
            {
                string[] paramDict = param.Split('=');
                if (paramDict[0].ToUpper() == "CARRESERVATIONID")
                    carReservationId = Convert.ToInt64(paramDict[1]);
                if (paramDict[0].ToUpper() == "PRINTEDBY")
                    printedby = paramDict[1];
            }

            //Load Agency
            AgenciesManager ageMan = new AgenciesManager();
            agency = ageMan.GetByNumber(AgencyNumber);

            //Load Car Reservation
            CarReservationsManager carsMan = new CarReservationsManager();
            entity = carsMan.GetById(AgencyNumber, carReservationId);
            entity.ToDate = new DateTime(entity.ToDate.Year, entity.ToDate.Month, entity.ToDate.Day, entity.FromDate.Hour, entity.FromDate.Minute, entity.FromDate.Second);
            entity.Days = entity.ToDate.Subtract(entity.FromDate).Days;

            //Load Payments
            CarReservationPaymentsManager payMan = new CarReservationPaymentsManager();
            payments = payMan.Get(AgencyNumber, entity.Id);
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
        private ReportDataSource GetFirstDataSource()
        {
            long number = 0;
            string phone = entity.ClientPhone;
            if (long.TryParse(entity.ClientPhone, out number))
                phone = string.Format("{0:(###) ###-####}", number);
            PaymentReceipt dataSet = new PaymentReceipt();
            dataSet.Tables["HeaderTable"].Rows.Add(new object[] {
                printedby,
                agency.LogoFilename,
                agency.Phone,
                agency.Email,
                agency.Fax,
                entity.ClientFirstname + " " + entity.ClientLastname,
                phone,
                entity.FromDate.ToString("dd/MM/yyyy"),
                entity.ToDate.ToString("dd/MM/yyyy"),
                entity.Days,
                entity.CarCategoryName,
                entity.SalePrice * entity.Days - (entity.Discount * entity.Days)
            });

            ReportDataSource dataSource = new ReportDataSource("HeaderTable", dataSet.Tables["HeaderTable"]);
            return dataSource;
        }
        private ReportDataSource GetSecondDataSource()
        {
            PaymentReceipt dataSet = new PaymentReceipt();
            foreach (PaymentExtension payment in payments) {
                if (!payment.AffectFinalPrice) continue;
                var amount = payment.IsReimbursement ? payment.Amount * -1 : payment.Amount;
                dataSet.Tables["PaymentTable"].Rows.Add(new object[] {
                    payment.CreatedOn.ToString("dd/MM/yyyy"),
                    amount,
                    payment.MethodName,
                    payment.ConceptName
                });
            }

            ReportDataSource dataSource = new ReportDataSource("Payment", dataSet.Tables["PaymentTable"]);
            return dataSource;
        }
        private object GetReport(string Format) {
            //Initialize Report
            string rootPath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "bin\\Reports\\");
            //string rootPath = @"C:\Users\Administrator\Source\Workspaces\StardomProject\Solution\Cars.Reports\Reports\";
            LocalReport report = new LocalReport();
            report.EnableExternalImages = true;
            report.ReportPath = rootPath + "PaymentReceipt.rdlc";
            report.DataSources.Add(GetFirstDataSource());
            report.DataSources.Add(GetSecondDataSource());
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
