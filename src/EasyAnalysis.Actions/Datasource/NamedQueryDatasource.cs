using EasyAnalysis.Framework.ConnectionStringProviders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;

namespace EasyAnalysis.Actions
{
    public class NamedQueryDatasource : IDisposable
    {
        private IConnectionStringProvider _connectionStringProvider;

        private readonly string _dbName;

        private readonly string _queryName;

        private SqlConnection _connection;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">dbname.query_n ame</param>
        public NamedQueryDatasource(string expression)
        {
            var temp = expression.Split('.');

            _dbName = temp[0];

            _queryName = temp[1];

            _connectionStringProvider = new UniversalConnectionStringProvider();

            _connection = new SqlConnection(_connectionStringProvider.GetConnectionString(_dbName));
        }

        public DbConnection Connection {
            get {
                return _connection;
            }
        }


        public IEnumerable<dynamic> Query(object param)
        {
            return _connection.Query(SqlQueryFactory.Instance.Get(_queryName), param);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
