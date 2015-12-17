using System;
using Dapper;
using System.Data.Common;
using EasyAnalysis.Framework.Data;

namespace EasyAnalysis.Data
{
    public class NamedQueryReadOnlyCollection : IReadOnlyCollection
    {
        private readonly string _queryName;

        private DbConnection _connection;

        private object _parameters;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">query_n ame</param>
        public NamedQueryReadOnlyCollection(string name, DbConnection connection)
        {
            _queryName = name;

            _connection = connection;
        }

        public DbConnection Connection {
            get {
                return _connection;
            }
        }

        public void SetParameters(object parameters)
        {
            _parameters = parameters;
        }

        public Object GetData()
        {
            return _connection.Query(SqlQueryFactory.Instance.Get(_queryName), _parameters);
        }
    }
}
