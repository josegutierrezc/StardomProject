using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class TourOperatorsManager
    {
        public TourOperator GetById(string AgencyNumber, int Id)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                return DB.TourOperators.Where(e => e.Id == Id).FirstOrDefault();
        }
        public List<CarCategory> GetCarCategories(string AgencyNumber, int TourOperatorId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from tocp in DB.TourOperatorCarProviders
                            join cpcc in DB.CarProvidersCarCategories on tocp.CarProviderId equals cpcc.CarProviderId
                            join cc in DB.CarCategories on cpcc.CarCategoryId equals cc.Id
                            where tocp.TourOperatorId == TourOperatorId
                            select cc;
                return query.OrderBy(e => e.Name).ToList();
            }
        }
        public List<TourOperator> Get(string AgencyNumber, bool OnlyActives) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
                if (OnlyActives)
                    return DB.TourOperators.Where(e => e.IsActive).ToList();
                else
                    return DB.TourOperators.ToList();
        }
        public List<TourOperator> Get(string AgencyNumber, int CarProviderId)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from tocp in DB.TourOperatorCarProviders
                            join to in DB.TourOperators on tocp.TourOperatorId equals to.Id
                            where tocp.CarProviderId == CarProviderId
                            select to;
                return query.ToList();
            }
                
        }
        public string GetImageFilename(string AgencyNumber, int TourOperatorId) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from to in DB.TourOperators
                            join i in DB.Images on to.ImageId equals i.Id
                            where to.Id == TourOperatorId
                            select i;
                return query.FirstOrDefault().Path;
            }
        }
    }
}
