using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class PrivilegesManager
    {
        private CarsModel DB;
        public PrivilegesManager(CarsModel DB) {
            this.DB = DB;
        }
        public List<Privilege> GetAll() {
            return DB.Privileges.ToList();
        }
        public List<Privilege> GetByUser(int UserId) {
            var query = from up in DB.UserPrivileges
                        join p in DB.Privileges on up.PrivilegeId equals p.Id
                        where up.UserId == UserId
                        select p;
            return query.ToList();
        }
        public bool AssignToUser(int UserId, string PrivilegeName) {
            var query = from up in DB.UserPrivileges
                        join p in DB.Privileges on up.PrivilegeId equals p.Id
                        where up.UserId == UserId & p.Name.ToUpper() == PrivilegeName.ToUpper()
                        select up;
            if (query.Count() != 0) return true;

            Privilege privilege = DB.Privileges.Where(e => e.Name.ToUpper() == PrivilegeName.ToUpper()).FirstOrDefault();
            if (privilege == null) return false;

            UserPrivilege userpri = new UserPrivilege();
            userpri.UserId = UserId;
            userpri.PrivilegeId = privilege.Id;
            DB.UserPrivileges.Add(userpri);
            DB.SaveChanges();

            return true;
        }
        public bool UnassignFromUser(int UserId, string PrivilegeName)
        {
            var query = from up in DB.UserPrivileges
                        join p in DB.Privileges on up.PrivilegeId equals p.Id
                        where up.UserId == UserId & p.Name.ToUpper() == PrivilegeName.ToUpper()
                        select up;

            if (query.Count() == 0) return true;

            DB.UserPrivileges.Remove(query.FirstOrDefault());
            DB.SaveChanges();

            return true;
        }
    }
}
