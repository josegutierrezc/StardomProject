using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class PaymentExtension
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUser { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public int ModifiedByUserId { get; set; }
        public int ReservationId { get; set; }
        public int ConceptId { get; set; }
        public string ConceptName { get; set; }
        public int MethodId { get; set; }
        public string MethodName { get; set; }
        public double Amount { get; set; }
        public bool IsReimbursement { get; set; }
        public bool AffectFinalPrice { get; set; }
    }
}
