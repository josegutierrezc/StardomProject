using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class PriceConfigurationsManager
    {
        public PriceConfiguration Get(string AgencyNumber, int CarCategoryId, int TourOperatorId, int SeasonId, int ReservationDayId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.PriceConfigurations.Where(pc => pc.CarCategoryId == CarCategoryId & pc.TourOperatorId == TourOperatorId & pc.SeasonId == SeasonId & pc.ReservationDayId == ReservationDayId).FirstOrDefault();
        }
        public List<PriceConfiguration> GetByTourOperatorAndSeason(string AgencyNumber, int TourOperatorId, int SeasonId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                return DB.PriceConfigurations.Where(e => e.TourOperatorId == TourOperatorId & e.SeasonId == SeasonId).ToList();
            }
        }
        public bool Update(string AgencyNumber, int TourOperatorId, int SeasonId, int ModifiedByUserId, List<PriceConfiguration> Configuration) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                DB.PriceConfigurations.RemoveRange(DB.PriceConfigurations.Where(e => e.TourOperatorId == TourOperatorId & e.SeasonId == SeasonId));
                DB.SaveChanges();
                foreach (PriceConfiguration pc in Configuration) {
                    pc.ModifiedBy = ModifiedByUserId;
                    pc.ModifiedOn = DateTime.Now;
                    DB.PriceConfigurations.Add(pc);
                }
                DB.SaveChanges();
                return true;
            }
        }
    }
}
