using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsManager.DL
{
    public class UsersManager
    {
        public List<User> GetAll() {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
                return DB.Users.ToList();
        }
        public User GetByUsername(string Username) {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
                return DB.Users.Where(e => e.Username.ToUpper() == Username.ToUpper()).FirstOrDefault();
        }
        public User GetById(int Id) {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
                return DB.Users.Where(e => e.Id == Id).FirstOrDefault();
        }
        public List<UserAgencyHelper> GetAgencies(string Username) {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
            {
                var query = from ua in DB.UserAgencies
                            join u in DB.Users on ua.UserId equals u.Id
                            join a in DB.Agencies on ua.AgencyId equals a.Id
                            where u.Username.ToUpper() == Username.ToUpper()
                            select new UserAgencyHelper {
                                Username = u.Username,
                                AgencyNumber = a.Number,
                                AgencyName = a.Name
                            };
                return query.ToList();
            }    
        }
        public List<UserAgencyHelper> GetUsers(string AgencyNumber) {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
            {
                var query = from ua in DB.UserAgencies
                            join u in DB.Users on ua.UserId equals u.Id
                            join a in DB.Agencies on ua.AgencyId equals a.Id
                            where a.Number == AgencyNumber
                            select new UserAgencyHelper
                            {
                                Firstname = u.FirstName,
                                Lastname = u.LastName,
                                Username = u.Username,
                                IsActive = u.IsActive,
                                AgencyNumber = a.Number
                            };
                return query.ToList();
            }
        }
        public bool Add(User Entity) {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                var query = DB.Users.Where(e => e.Username.ToUpper() == Entity.Username.ToUpper());
                if (query.Count() != 0) return false;

                DB.Users.Add(Entity);
                DB.SaveChanges();
                return true;
            }
        }
        public bool Update(User Entity) {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
            {
                User user = DB.Users.Where(e => e.Username.ToUpper() == Entity.Username.ToUpper()).FirstOrDefault();
                if (user == null) return false;

                user.FirstName = Entity.FirstName;
                user.LastName = Entity.LastName;
                user.IsActive = Entity.IsActive;
                DB.SaveChanges();

                return true;
            }
        }
        public bool Delete(string Username) {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
            {
                User user = DB.Users.Where(e => e.Username.ToUpper() == Username.ToUpper()).FirstOrDefault();
                if (user == null) return false;

                DB.Users.Remove(user);
                DB.SaveChanges();
                return true;
            }
        }
        public bool AssignAgency(UserAgencyHelper Entity) {
            using (var DB = AppsManagerModel.ConnectToSqlServer()) {
                var query = from ua in DB.UserAgencies
                            join u in DB.Users on ua.UserId equals u.Id
                            join a in DB.Agencies on ua.AgencyId equals a.Id
                            where u.Username.ToUpper() == Entity.Username.ToUpper() & a.Number == Entity.AgencyNumber
                            select ua;
                if (query.Count() != 0) return true;

                User user = DB.Users.Where(e => e.Username.ToUpper() == Entity.Username).FirstOrDefault();
                if (user == null) return false;

                Agency agency = DB.Agencies.Where(e => e.Number == Entity.AgencyNumber).FirstOrDefault();
                if (agency == null) return false;

                UserAgency useragency = new UserAgency();
                useragency.UserId = user.Id;
                useragency.AgencyId = agency.Id;
                DB.UserAgencies.Add(useragency);
                DB.SaveChanges();

                return true;
            }
        }
        public bool UnassignAgency(string Username, string AgencyNumber)
        {
            using (var DB = AppsManagerModel.ConnectToSqlServer())
            {
                var query = from ua in DB.UserAgencies
                            join u in DB.Users on ua.UserId equals u.Id
                            join a in DB.Agencies on ua.AgencyId equals a.Id
                            where u.Username.ToUpper() == Username.ToUpper() & a.Number == AgencyNumber
                            select ua;
                if (query.Count() != 1) return false;

                DB.UserAgencies.Remove(query.FirstOrDefault());
                DB.SaveChanges();
                
                return true;
            }
        }
    }
}
