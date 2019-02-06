using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsManager.DL
{
    public class AgenciesManager
    {
        private AppsManagerModel DB;

        public AgenciesManager(AppsManagerModel DB) {
            this.DB = DB;
        }

        public List<Agency> GetAll()
        {
            return DB.Agencies.ToList();
        }

        public Agency GetByNumber(string Number)
        {
            return DB.Agencies.Where(e => e.Number == Number).FirstOrDefault(); 
        }

        public bool Add(Agency Entity) {
            var match = DB.Agencies.Where(e => e.Number.ToUpper() == Entity.Number.ToUpper());
            if (match.Count() != 0) return false;

            DB.Agencies.Add(Entity);
            DB.SaveChanges();
            return true;
        }

        public bool Delete(string Number) {
            if (Number == "0000") return false;

            var match = DB.Agencies.Where(e => e.Number.ToUpper() == Number.ToUpper()).FirstOrDefault();
            if (match == null) return false;

            DB.Agencies.Remove(match);
            DB.SaveChanges();
            return true;
        }

        public bool Update(string Number, Agency Entity)
        {
            if (Number == "0000") return false;

            var match = DB.Agencies.Where(e => e.Number.ToUpper() == Number.ToUpper()).FirstOrDefault();
            if (match == null) return false;

            match.Name = Entity.Name;
            match.Address = Entity.Address;
            match.City = Entity.City;
            match.State = Entity.State;
            match.ZipCode = Entity.ZipCode;
            match.ContactName = Entity.ContactName;
            match.Phone = Entity.Phone;
            match.LogoFilename = Entity.LogoFilename;
            match.IsActive = Entity.IsActive;
            match.VoucherNotes = Entity.VoucherNotes;
            match.RentMEVersion = Entity.RentMEVersion;
            match.Disclaimer = Entity.Disclaimer;
            DB.SaveChanges();

            return true;
        }
    }
}
