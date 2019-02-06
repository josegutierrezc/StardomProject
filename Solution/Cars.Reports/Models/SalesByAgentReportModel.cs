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
    public class SalesByAgentReportModel : ReportModel
    {
        #region Constructor
        public SalesByAgentReportModel() {
            Name = "Ventas por agente";
            Url = "agentsales";
        }
        #endregion

        #region Private Variables
        private Agency agency;
        private DateTime fromDate;
        private DateTime toDate;
        private string printedby;
        SortedDictionary<string, SortedDictionary<string, SaleAmounts>> sales;
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

            //Load Payment Methods
            UsersManager userMan = new UsersManager();
            PaymentMethodsManager payMan = new PaymentMethodsManager();
            List<AgentSalesSummary> salesbyAgent = payMan.GetReportOfSalesByAgent(AgencyNumber, fromDate, toDate);
            sales = new SortedDictionary<string, SortedDictionary<string, SaleAmounts>>();
            foreach (AgentSalesSummary sale in salesbyAgent) {
                User u = userMan.GetById(sale.AgentId);
                DateTime toDate = new DateTime(sale.ToDate.Year, sale.ToDate.Month, sale.ToDate.Day, sale.FromDate.Hour, sale.FromDate.Minute, sale.FromDate.Second);
                int days = toDate.Subtract(sale.FromDate).Days;
                double potentialAmount = sale.SalePrice * days - sale.Discount * days;

                sale.AgentFullname = u.FirstName + " " + u.LastName;
                sale.Days = days;
                sale.PotentialAmount = potentialAmount;

                SortedDictionary<string, SaleAmounts> agentAmounts;
                if (!sales.TryGetValue(sale.AgentFullname, out agentAmounts)) {
                    agentAmounts = new SortedDictionary<string, SaleAmounts>();
                    sales.Add(sale.AgentFullname, agentAmounts);
                }

                if (agentAmounts.ContainsKey(sale.PaymentMethodName))
                {
                    agentAmounts[sale.PaymentMethodName].PotentialAmount += sale.PotentialAmount;
                    agentAmounts[sale.PaymentMethodName].PotentialAmount += sale.PaidAmount;
                }
                else
                    agentAmounts.Add(sale.PaymentMethodName, new SaleAmounts()
                    {
                        PotentialAmount = sale.PotentialAmount,
                        PaidAmount = sale.PaidAmount
                    });
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
            SalesByAgent dataSet = new SalesByAgent();
            dataSet.Tables["HeaderTable"].Rows.Add(new object[] {
                fromDate.ToString("dd/MM/yyyy"),
                toDate.ToString("dd/MM/yyyy"),
                agency.LogoFilename,
                printedby
            });

            ReportDataSource dataSource = new ReportDataSource("Header", dataSet.Tables["HeaderTable"]);
            return dataSource;
        }
        private ReportDataSource GetCarReservationsDataSource() {
            SalesByAgent dataSet = new SalesByAgent();
            foreach (KeyValuePair<string, SortedDictionary<string, SaleAmounts>> kvp in sales)
            {
                string agentfullname = kvp.Key;
                foreach (KeyValuePair<string, SaleAmounts> kvp2 in kvp.Value) {
                    string paymentmethodname = kvp2.Key;
                    double potentialamount = kvp2.Value.PotentialAmount;
                    double paidamount = kvp2.Value.PaidAmount;
                    dataSet.Tables["DetailsTable"].Rows.Add(new object[] {
                    agentfullname,
                    paymentmethodname,
                    potentialamount,
                    paidamount
                });
                }
            }
            ReportDataSource dataSource = new ReportDataSource("SaleDetails", dataSet.Tables["DetailsTable"]);
            return dataSource;
        }
        private object GetReport(string Format) {
            //Initialize Report
            string rootPath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "bin\\Reports\\");
            //string rootPath = @"C:\Users\Administrator\Source\Workspaces\StardomProject\Solution\Cars.Reports\Reports\";
            LocalReport report = new LocalReport();
            report.EnableExternalImages = true;
            report.ReportPath = rootPath + "SalesByAgent.rdlc";
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

    public class SaleAmounts
    {
        public double PotentialAmount { get; set; }
        public double PaidAmount { get; set; }
    }
}
