using System.Configuration;
using System.Data;
using System.Data.Common;

namespace DAL
{
    public static class DataFactory
    {
        public static IDbConnection CreateConnection()
        {
            var connectionStringItem = ConfigurationManager.ConnectionStrings["NorthwindConnection"];
            var connectionString = connectionStringItem.ConnectionString;
            var provider = connectionStringItem.ProviderName;

            var factory = DbProviderFactories.GetFactory(provider);
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
