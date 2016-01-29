using EasyAnalysis.Framework.ConnectionStringProviders;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace EasyAnalysis.Data
{
    public class SqlNamedQueryCollection<TModel> : Framework.Data.IReadOnlyCollection<TModel>
    {
        private string _queryName;

        private string _connectionName;

        private object _parameters;

        private IConnectionStringProvider _connectionStringProvider = new UniversalConnectionStringProvider();

        public SqlNamedQueryCollection(string connectionName, string queryName)
        {
            _connectionName = connectionName;

            _queryName = queryName;
        }

        public void SetParameters(object parameters)
        {
            _parameters = parameters;
        }

        public Task ForEachAsync(Action<TModel> processor)
        {
            return Task.Run(() => {
                var sqlQueryText = SqlQueryFactory.Instance.Get(_queryName);

                using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString(_connectionName)))
                {
                    var colection = connection.Query<TModel>(sqlQueryText, _parameters);

                    foreach (TModel item in colection)
                    {
                        processor(item);
                    }
                }
            });
        }

        public Task ForEachAsync(Func<TModel, Task> processor)
        {
            return Task.Run(async () => {
                var sqlQueryText = SqlQueryFactory.Instance.Get(_queryName);

                using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString(_connectionName)))
                {
                    var colection = connection.Query<TModel>(sqlQueryText, _parameters);

                    foreach (TModel item in colection)
                    {
                        await processor(item);
                    }
                }
            });
        }
    }
}
