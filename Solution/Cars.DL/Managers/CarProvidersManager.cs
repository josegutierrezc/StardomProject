using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class CarProvidersManager
    {
        public List<CarProvider> Get(string AgencyNumber) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.CarProviders.ToList();
        }
    }
}
