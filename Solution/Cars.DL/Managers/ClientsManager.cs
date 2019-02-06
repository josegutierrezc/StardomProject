using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public class ClientsManager
    {
        public KeyValuePair<int, List<ClientExtension>> GetAll(string AgencyNumber, int PageSize, int PageNumber, string SearchFor)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                List<ClientExtension> result;
                int totalitems = 0;
                var query = from c in DB.Clients
                            join a in DB.LinkedAgencies on c.LinkedToAgencyId equals a.Id into la
                            from linka in la.DefaultIfEmpty()
                            select new ClientExtension
                            {
                                Birthday = c.Birthday,
                                CreatedByUserId = c.CreatedByUserId,
                                CreatedOn = c.CreatedOn,
                                Email = c.Email,
                                FirstName = c.FirstName,
                                Id = c.Id,
                                LastName = c.LastName,
                                LinkedToAgencyId = c.LinkedToAgencyId,
                                LinkedAgencyName = linka == null ? string.Empty : linka.Name,
                                ModifiedByUserId = c.ModifiedByUserId,
                                ModifiedOn = c.ModifiedOn,
                                Notes = c.Notes,
                                Phone = c.Phone,
                                Troubled = c.Troubled
                            };
                if (SearchFor == null || SearchFor == string.Empty)
                {
                    query = query.OrderBy(e => e.FirstName).ThenBy(e => e.LastName);
                    totalitems = query.Count();
                    result = query.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                }
                else
                {
                    query = query.Where(e => e.FirstName.Contains(SearchFor) | e.LastName.Contains(SearchFor) | e.Phone.Contains(SearchFor) | e.Email.Contains(SearchFor) | e.LinkedAgencyName.Contains(SearchFor) | (e.FirstName + " " + e.LastName).Contains(SearchFor)).OrderBy(e => e.FirstName).ThenBy(e => e.LastName);
                    totalitems = query.Count();
                    result = query.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                }

                return new KeyValuePair<int, List<ClientExtension>>(totalitems, result);
            }
        }
        public ClientExtension GetById(string AgencyNumber, int Id) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var query = from c in DB.Clients
                            join a in DB.LinkedAgencies on c.LinkedToAgencyId equals a.Id into la
                            from linka in la.DefaultIfEmpty()
                            where c.Id == Id
                            select new ClientExtension
                            {
                                Birthday = c.Birthday,
                                CreatedByUserId = c.CreatedByUserId,
                                CreatedOn = c.CreatedOn,
                                Email = c.Email,
                                FirstName = c.FirstName,
                                Id = c.Id,
                                LastName = c.LastName,
                                LinkedToAgencyId = c.LinkedToAgencyId,
                                LinkedAgencyName = linka == null ? string.Empty : linka.Name,
                                ModifiedByUserId = c.ModifiedByUserId,
                                ModifiedOn = c.ModifiedOn,
                                Notes = c.Notes,
                                Phone = c.Phone,
                                Troubled = c.Troubled
                            };
                return query.FirstOrDefault();
            }
        }
        public bool Remove(string AgencyNumber, int Id) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                var entity = DB.Clients.Where(e => e.Id == Id).FirstOrDefault();
                if (entity == null) return false;

                var reservations = DB.CarReservations.Where(e => e.ClientId == Id);
                if (reservations.Count() != 0) return false;

                DB.Clients.Remove(entity);
                DB.SaveChanges();
                return true;
            }
        }
        public bool Update(string AgencyNumber, ClientExtension Entity)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber))
            {
                var entity = DB.Clients.Where(e => e.Id == Entity.Id).FirstOrDefault();
                if (entity == null) return false;

                entity.ModifiedByUserId = Entity.ModifiedByUserId;
                entity.ModifiedOn = DateTime.Now;
                entity.FirstName = Entity.FirstName;
                entity.LastName = Entity.LastName;
                entity.Phone = Entity.Phone;
                entity.Email = Entity.Email;
                entity.Birthday = Entity.Birthday;
                entity.Notes = Entity.Notes;
                entity.LinkedToAgencyId = Entity.LinkedToAgencyId;
                entity.Troubled = Entity.Troubled;

                DB.SaveChanges();

                return true;
    }
        }
        public int Add(string AgencyNumber, Client Entity) {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                Entity.CreatedOn = DateTime.Now;
                DB.Clients.Add(Entity);
                DB.SaveChanges();
                return Entity.Id;
            }
        }
        public bool FormatPhoneNumber(string AgencyNumber)
        {
            using (var DB = CarsModel.ConnectToSqlServer(AgencyNumber)) {
                foreach (Client c in DB.Clients) {
                    if (c.Phone == null || c.Phone == string.Empty) continue;

                    string original = c.Phone;

                    int pointer = 0;
                    while (c.Phone.Length != 0 & pointer <= c.Phone.Length - 1)
                        if (char.IsNumber(c.Phone[pointer]))
                            pointer += 1;
                        else
                            c.Phone = c.Phone.Remove(pointer, 1);

                    long number;
                    if (!long.TryParse(c.Phone, out number))
                    {
                        c.Phone = string.Empty;
                        c.Notes += original;
                    }
                    else if (c.Phone.Length != 10) {
                        c.Phone = string.Empty;
                        c.Notes += original;
                    }                        
                }
                DB.SaveChanges();
                return true;
            }
        }
    }
}
