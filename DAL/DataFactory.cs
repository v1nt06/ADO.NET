using System.Data;
using System.Data.Common;

namespace DAL
{
    public static class DataFactory
    {
        public static IDbConnection CreateConnection(string connectionString, string provider)
        {
            var factory = DbProviderFactories.GetFactory(provider);
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
