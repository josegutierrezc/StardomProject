using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class CarCategoriesManager
    {
        public List<CarCategory> Get(string AgencyNumber) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.CarCategories.ToList();
        }
        public List<CarCategory> Get(string AgencyNumber, int CarProviderId)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                var query = from cpcc in DB.CarProvidersCarCategories
                            join cc in DB.CarCategories on cpcc.CarCategoryId equals cc.Id
                            where cpcc.CarProviderId == CarProviderId
                            select cc;
                return query.ToList();
            }
        }
    }
}
