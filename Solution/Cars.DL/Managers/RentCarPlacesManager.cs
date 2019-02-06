using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class RentCarPlacesManager
    {
        public List<RentCarPlace> Get(string AgencyNumber, int ProvinceId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.RentCarPlaces.Where(e => e.ProvinceId == ProvinceId).ToList();
        }
        public List<RentCarPlace> GetActives(string AgencyNumber, int ProvinceId)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.RentCarPlaces.Where(e => e.ProvinceId == ProvinceId & e.IsActive).ToList();
        }
    }
}
