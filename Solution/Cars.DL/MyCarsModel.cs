using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DL
{
    public partial class CarsModel : DbContext
    {
        public CarsModel(string connectionString)
            : base(connectionString)
        {
        }
        public static CarsModel ConnectToSqlServer(string AgencyNumber)
        {
#if (DEBUG)
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"DEVSERVER\SQLEXPRESS",
                InitialCatalog = "Cars-" + AgencyNumber,
                MultipleActiveResultSets = true,

                UserID = "rentmeadmin",
                Password = "1KillsAll",
            };
#else
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"SERVER\SQLSERVER",
                InitialCatalog = "Cars-" + AgencyNumber,
                MultipleActiveResultSets = true,

                UserID = "rentmeadmin",
                Password = "1KillsAll",
            };
#endif

            // assumes a connectionString name in .config of MyDbEntities
            var entityConnectionStringBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ConnectionString,
                Metadata = "res://*/CarsModel.csdl|res://*/CarsModel.ssdl|res://*/CarsModel.msl",
            };

            return new CarsModel(entityConnectionStringBuilder.ConnectionString);
        }
    }
}
