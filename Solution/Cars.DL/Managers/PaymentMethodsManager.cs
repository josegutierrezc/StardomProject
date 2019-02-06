using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class PaymentMethodsManager
    {
        public List<PaymentMethod> Get(string AgencyNumber) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.PaymentMethods.ToList();
        }
        public Dictionary<string, double> GetReportOfPaymentTypesPerPeriod(string AgencyNumber, DateTime FromDate, DateTime ToDate) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from p in DB.Payments
                            join m in DB.PaymentMethods on p.MethodId equals m.Id
                            join cr in DB.CarReservations on p.ReservationId equals cr.Id
                            where cr.FromDate >= FromDate & cr.FromDate <= ToDate & cr.CancelledOn == null
                            select new {
                                PaymentMethodName = m.Name,
                                Amount = p.Amount
                            };
                Dictionary<string, double> dict = new Dictionary<string, double>();
                foreach (var elem in query) {
                    if (dict.ContainsKey(elem.PaymentMethodName))
                        dict[elem.PaymentMethodName] += elem.Amount;
                    else
                        dict.Add(elem.PaymentMethodName, elem.Amount);
                }
                return dict;
            }
        }
        public List<AgentSalesSummary> GetReportOfSalesByAgent(string AgencyNumber, DateTime FromDate, DateTime ToDate) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                var query = from p in DB.Payments
                            join m in DB.PaymentMethods on p.MethodId equals m.Id
                            join cr in DB.CarReservations on p.ReservationId equals cr.Id
                            where cr.FromDate >= FromDate & cr.FromDate <= ToDate & cr.CancelledOn == null
                            select new AgentSalesSummary
                            {
                                PaymentMethodName = m.Name,
                                AgentId = cr.CreatedBy,
                                AgentFullname = string.Empty,
                                FromDate = cr.FromDate,
                                ToDate = cr.ToDate,
                                SalePrice = cr.SalePrice == null ? 0 : (double)cr.SalePrice,
                                Discount = cr.Discount == null ? 0 : (double)cr.Discount,
                                PotentialAmount = 0,
                                PaidAmount = p.Amount
                            };
                return query.ToList();
            }
        }
    }
}
