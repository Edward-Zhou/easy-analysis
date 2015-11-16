using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace EasyAnalysis.Infrastructure.Cache
{
    internal class CacheDataContext : IDisposable
    {
        private SQLiteConnection _connection;

        public CacheDataContext(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public void Dispose()
        {
           
        }

        public CacheIndex GetCacheIndex(string hash)
        {
            var selectByHashSql = SqlLibrary.Instance.Require("SELECT_INDEX_BY_HASH");

            var cacheIndex = _connection
                                .Query<CacheIndex>(selectByHashSql, new { @Hash = hash })
                                .FirstOrDefault();

            return cacheIndex;
        }

        public void DelteCacheIndex(string hash)
        {
            var deleteByHashSql = SqlLibrary.Instance.Require("DELETE_INDEX_BY_HASH");

            _connection.Execute(deleteByHashSql, new { Hash = hash });
        }

        public void InsertCaccheIndex(CacheIndex index)
        {
            var insertIntoCacheSql = SqlLibrary.Instance.Require("INSERT_INTO_CACHE_INDEX");

            _connection.Execute(insertIntoCacheSql, index);
        }
    }
}
