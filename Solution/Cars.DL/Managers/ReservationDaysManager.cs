using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class ReservationDaysManager
    {
        public List<ReservationDay> GetAll(string AgencyNumber)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                return DB.ReservationDays.ToList();
            }
        }
        public ReservationDay Get(string AgencyNumber, int CarCategoryId, int NumberOfDays) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var result = from rd in DB.ReservationDays
                             join ccrd in DB.CarCategoryReservationDays on rd.Id equals ccrd.ReservationDayId
                             where NumberOfDays >= rd.FromDay & NumberOfDays <= rd.ToDay & ccrd.CarCategoryId == CarCategoryId
                             select rd;
                return result.FirstOrDefault();
            }
        }
        public ReservationDay GetMax(string AgencyNumber, int CarCategoryId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var maxDayQuery = from rd in DB.ReservationDays
                             orderby rd.ToDay descending
                             select rd;
                var maxDay = maxDayQuery.FirstOrDefault().ToDay;

                var result = from rd in DB.ReservationDays
                             join ccrd in DB.CarCategoryReservationDays on rd.Id equals ccrd.ReservationDayId
                             where rd.ToDay == maxDay & ccrd.CarCategoryId == CarCategoryId
                             select rd;
                return result.FirstOrDefault();
    }
        }
    }
}
