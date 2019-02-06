using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class AgentSalesSummary
    {
        public string PaymentMethodName { get; set; }
        public int AgentId { get; set; }
        public string AgentFullname { get; set; }
        public double SalePrice { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Days { get; set; }
        public double Discount { get; set; }
        public double PotentialAmount { get; set; }
        public double PaidAmount { get; set; }
    }
}
