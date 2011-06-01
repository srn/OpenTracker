using System.Configuration;

namespace OpenTracker.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenTrackerDbContext : OpenTrackerDb
    {
        public OpenTrackerDbContext()
            : base(ConfigurationBuilder.BuildConnectionString())
        { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConfigurationBuilder
    {
        private const string APP_CONFIG_KEY = "CONNECTIONSTRING";
        private const string EDMX_PATH = "OpenTrackerEntities";

        public static string BuildConnectionString()
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[APP_CONFIG_KEY]))
                throw new ConfigurationErrorsException(APP_CONFIG_KEY + " was not found in Application Settings");

            var conn = ConfigurationManager.AppSettings[APP_CONFIG_KEY];

            if (ConfigurationManager.ConnectionStrings[conn] == null)
                throw new ConfigurationErrorsException(conn + " was not found in Connection Strings");

            var _connectionString = ConfigurationManager.ConnectionStrings[conn].ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
                throw new ConfigurationErrorsException(conn + " cannot have an empty connection string");

            const string META_DATA = @"metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=""{1};Persist Security Info=True""";
            return string.Format(META_DATA, EDMX_PATH, _connectionString);
        }
    }
}
