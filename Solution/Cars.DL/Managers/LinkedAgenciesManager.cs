using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class LinkedAgenciesManager
    {
        public List<LinkedAgency> Get(string AgencyNumber)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                return DB.LinkedAgencies.OrderBy(e => e.Name).ToList();
            }
        }
        public int Add(string AgencyNumber, LinkedAgency Entity)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                LinkedAgency entity = new LinkedAgency();
                entity.Name = Entity.Name;
                entity.ContactName = Entity.ContactName;
                entity.Phone = Entity.Phone;
                entity.Email = Entity.Email;
                DB.LinkedAgencies.Add(entity);

                DB.SaveChanges();
                return entity.Id;
            }
        }
        public bool Update(string AgencyNumber, LinkedAgency Entity) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                LinkedAgency entity = DB.LinkedAgencies.Where(e => e.Id == Entity.Id).FirstOrDefault();
                if (entity == null) return false;

                entity.Name = Entity.Name;
                entity.ContactName = Entity.ContactName;
                entity.Phone = Entity.Phone;
                entity.Email = Entity.Email;

                DB.SaveChanges();
                return true;
            }
        }
    }
}
