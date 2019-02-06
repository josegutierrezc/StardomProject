using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class CarReservationPaymentsManager
    {
        public List<PaymentExtension> Get(string AgencyNumber, long CarReservationId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from p in DB.Payments
                            join c in DB.PaymentConcepts on p.ConceptId equals c.Id
                            join m in DB.PaymentMethods on p.MethodId equals m.Id
                            where p.ReservationId == CarReservationId
                            orderby p.CreatedOn descending
                            select new PaymentExtension {
                                Id = p.Id,
                                ReservationId = p.ReservationId,
                                CreatedByUserId = p.CreatedByUserId,
                                CreatedOn = p.CreatedOn,
                                ModifiedByUserId = p.ModifiedByUserId,
                                ModifiedOn = p.ModifiedOn,
                                ConceptId = p.ConceptId,
                                ConceptName = c.Name,
                                MethodId = p.MethodId,
                                MethodName = m.Name,
                                Amount = p.Amount,
                                IsReimbursement = p.IsReimbursement,
                                AffectFinalPrice = c.AffectFinalPrice
                            };
                return query.ToList();
            }
        }

        public double GetTotalPaid(string AgencyNumber, long CarReservationId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from p in DB.Payments
                            join pc in DB.PaymentConcepts on p.ConceptId equals pc.Id
                            where p.ReservationId == CarReservationId
                            select new
                            {
                                Payment = p,
                                Concept = pc
                            };
                double total = 0;
                foreach (var elem in query) {
                    if (elem.Concept.AffectFinalPrice)
                    {
                        if (elem.Payment.IsReimbursement)
                            total -= elem.Payment.Amount;
                        else
                            total += elem.Payment.Amount;
                    }
                }
                return total;
            }
        }

        public bool Add(string AgencyNumber, Payment Entity) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                Entity.CreatedOn = DateTime.Now;
                Entity.ModifiedByUserId = Entity.CreatedByUserId;
                Entity.ModifiedOn = Entity.CreatedOn;
                DB.Payments.Add(Entity);
                DB.SaveChanges();
                return true;
            }
        }

        public bool Delete(string AgencyNumber, int PaymentId)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                var entity = DB.Payments.Where(e => e.Id == PaymentId).FirstOrDefault();
                if (entity == null) return false;

                DB.Payments.Remove(entity);
                DB.SaveChanges();
                return true;
            }
        }

        public PaymentsPerPeriod GetTotalPaymentsPerPeriodComparison(string AgencyNumber, DateTime FromDate, int TotalDays, int ComparedWithYear) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var fromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
                var toDate = fromDate.AddDays(TotalDays);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

                var firstquery = from p in DB.Payments
                            join pc in DB.PaymentConcepts on p.ConceptId equals pc.Id
                            where pc.AffectFinalPrice & p.CreatedOn >= fromDate & p.CreatedOn <= toDate
                            select new
                            {
                                p.CreatedOn,
                                p.IsReimbursement,
                                p.Amount
                            };

                var secondFromDate = new DateTime(ComparedWithYear, FromDate.Month, FromDate.Day, FromDate.Hour, FromDate.Minute, FromDate.Second);
                var secondToDate = new DateTime(ComparedWithYear, toDate.Month, toDate.Day, toDate.Hour, toDate.Minute, toDate.Second);
                var secondquery = from p in DB.Payments
                                  join pc in DB.PaymentConcepts on p.ConceptId equals pc.Id
                                  where pc.AffectFinalPrice & p.CreatedOn >= secondFromDate & p.CreatedOn <= secondToDate
                                  select new
                                  {
                                      p.CreatedOn,
                                      p.IsReimbursement,
                                      p.Amount
                                  };

                PaymentsPerPeriod data = new PaymentsPerPeriod();
                data.FromDate = FromDate;
                data.TotalDays = TotalDays;
                data.ComparedWithYear = ComparedWithYear;
                data.TotalPaidFirstYear = new double[TotalDays + 1];
                data.TotalPaidSecondYear = new double[TotalDays + 1];
                int index = 0;
                for (int day = fromDate.Day; day <= fromDate.Day + TotalDays; day += 1)
                {
                    double totalPaid = 0;
                    foreach (var elem in firstquery)
                        if (elem.CreatedOn.Day == day) {
                            if (elem.IsReimbursement)
                                totalPaid -= elem.Amount;
                            else
                                totalPaid += elem.Amount;
                        }
                    data.TotalPaidFirstYear[index] = totalPaid;

                    totalPaid = 0;
                    foreach (var elem in secondquery)
                        if (elem.CreatedOn.Day == day)
                        {
                            if (elem.IsReimbursement)
                                totalPaid -= elem.Amount;
                            else
                                totalPaid += elem.Amount;
                        }
                    data.TotalPaidSecondYear[index] = totalPaid;

                    index += 1;
                }

                return data;
            }
        }
    }
}
