using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class PaymentsPerPeriod
    {
        public DateTime FromDate { get; set; }
        public int TotalDays { get; set; }
        public int ComparedWithYear { get; set; }
        public double[] TotalPaidFirstYear { get; set; }
        public double[] TotalPaidSecondYear { get; set; }
    }
}
