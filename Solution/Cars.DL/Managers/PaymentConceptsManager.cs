using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class PaymentConceptsManager
    {
        public List<PaymentConcept> Get(string AgencyNumber) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.PaymentConcepts.ToList();
        }
    }
}
