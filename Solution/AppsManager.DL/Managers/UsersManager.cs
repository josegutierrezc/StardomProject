using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppsManager.DL
{
    public class UsersManager
    {
        protected AppsManagerModel DB;
        public UsersManager(AppsManagerModel DB) {
            this.DB = DB;
        }
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
            var query = DB.Users.Where(e => e.Username.ToUpper() == Entity.Username.ToUpper());
            if (query.Count() != 0) return false;

            DB.Users.Add(Entity);
            DB.SaveChanges();
            return true;
        }
        public bool Update(User Entity) {
            User user = DB.Users.Where(e => e.Username.ToUpper() == Entity.Username.ToUpper()).FirstOrDefault();
            if (user == null) return false;

            MD5 md5Hash = MD5.Create();
            user.FirstName = Entity.FirstName;
            user.LastName = Entity.LastName;
            user.IsActive = Entity.IsActive;

            if (Entity.Password != null && Entity.Password != string.Empty)
                user.Password = Encrypt(md5Hash, Entity.Password);

            DB.SaveChanges();

            return true;
        }
        public bool Delete(string Username) {
            User user = DB.Users.Where(e => e.Username.ToUpper() == Username.ToUpper()).FirstOrDefault();
            if (user == null) return false;

            DB.Users.Remove(user);
            DB.SaveChanges();
            return true;
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

        #region Helpers
        public static string Encrypt(MD5 md5Hash, string plainText)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion
    }
}
