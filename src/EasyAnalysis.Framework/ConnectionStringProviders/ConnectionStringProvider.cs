using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.ConnectionStringProviders
{
    public class ConnectionStringProvider
    {
        /// <summary>
        /// ConnectionString Enum Type
        /// </summary>
        public enum ConnectionStringProviderType
        {
            MongoDBConnectionStringProvider = 0,
            MSSQLConnectionStringProvider = 1
        }

        /// <summary>
        /// Create the ConnectionString Provider
        /// </summary>
        /// <param name="ConnectionStringProviderType">ConnectionString Enum Type</param>
        /// <returns></returns>
        public static IConnectionStringProvider CreateConnectionStringProvider(ConnectionStringProviderType connectionStringProviderType)
        {
            switch (connectionStringProviderType)
            {
                case ConnectionStringProviderType.MongoDBConnectionStringProvider:
                    return new MongoDBConnectionStringProvider();
                case ConnectionStringProviderType.MSSQLConnectionStringProvider:
                    return new SqlServerConnectionStringProvider();
                default:
                    return null;
            }
        }
    }
}