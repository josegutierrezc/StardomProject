using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsManager.DL
{
    public partial class AppsManagerModel : DbContext
    {
        public AppsManagerModel(string connectionString)
            : base(connectionString)
        {
        }
        public static AppsManagerModel ConnectToSqlServer()
        {
#if (DEBUG)
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"DEVSERVER\SQLEXPRESS",
                InitialCatalog = "AppsManager",
                MultipleActiveResultSets = true,

                UserID = "rentmeadmin",
                Password = "1KillsAll",
            };
#else
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"SERVER\SQLSERVER",
                InitialCatalog = "AppsManager",
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
                Metadata = "res://*/AppsManagerModel.csdl|res://*/AppsManagerModel.ssdl|res://*/AppsManagerModel.msl",
            };

            return new AppsManagerModel(entityConnectionStringBuilder.ConnectionString);
        }
    }
}
