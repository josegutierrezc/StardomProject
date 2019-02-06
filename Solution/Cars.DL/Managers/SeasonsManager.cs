using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class SeasonsManager
    {
        public List<Season> GetAll(string AgencyNumber) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.Seasons.ToList();
        }
        public SeasonDate Get(string AgencyNumber, int FromDayNumber, int ToDayNumber) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.SeasonDates.Where(sd => FromDayNumber >= sd.FromDate & ToDayNumber <= sd.ToDate).FirstOrDefault();
        }

        public SeasonDate GetMax(string AgencyNumber, int FromDayNumber) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var result = from sd in DB.SeasonDates
                             where FromDayNumber >= sd.FromDate
                             orderby sd.FromDate descending
                             select sd;
                return result.FirstOrDefault();
            }
        }
    }
}
