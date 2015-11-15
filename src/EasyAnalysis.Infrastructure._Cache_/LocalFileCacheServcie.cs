using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SQLite;
using System.Diagnostics;
using EasyAnalysis.Framework.Cache;

namespace EasyAnalysis.Infrastructure.Cache
{
    public class CacheIndex
    {
        public string Url  { get; set; }

        public string Path { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ExpiredOn { get; set; }

        public string Hash { get; set; }
    }

    public class LocalFileCacheServcie : ICacheService
    {
        private bool _isInitialized = false;

        private string _rootFolder;

        private string _sqliteFilePath;

        public ICacheClient CreateClient()
        {
            Initialize();

            return new LocalFileCacheClient(this);
        }

        public void Configure(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public CacheStatus GetStatus(Uri uri)
        {
            var hash = Utils.ComputeStringMD5Hash(uri.AbsoluteUri, Encoding.UTF8);

            var selectByHashSql = SqlLibrary.Instance.Require("SELECT_INDEX_BY_HASH");

            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
            using (var context = new CacheDataContext(connection))
            {
                var cacheIndex = context.GetCacheIndex(hash);

                if (cacheIndex == null)
                {
                    return CacheStatus.None;
                }

                var cacheFilePath = GetCacheFilePhysicalPath(cacheIndex.Path);

                if (!File.Exists(cacheFilePath))
                {
                    return CacheStatus.Deleted;
                }

                if(cacheIndex.ExpiredOn < DateTime.Now)
                {
                    return CacheStatus.Expired;
                }

                return CacheStatus.Active;
            }

        }

        public Stream GetCache(Uri uri)
        {
            // compute hash
            var hash = Utils.ComputeStringMD5Hash(uri.AbsoluteUri, Encoding.UTF8);

            var selectByHashSql = SqlLibrary.Instance.Require("SELECT_INDEX_BY_HASH");

            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
            using (var context = new CacheDataContext(connection))
            {
                try
                {
                    var cacheIndex = context.GetCacheIndex(hash);

                    if (cacheIndex == null)
                    {
                        return null;
                    }

                    var cacheFilePath = GetCacheFilePhysicalPath(cacheIndex.Path);

                    if (!File.Exists(cacheFilePath))
                    {
                        return null;
                    }

                    var mem = new MemoryStream();

                    using (var fs = new FileStream(cacheFilePath, FileMode.Open))
                    {
                        fs.CopyTo(mem);

                        mem.Flush();

                        mem.Position = 0;
                    }

                    return mem;
                }
                catch (Exception ex)
                {
                    // TODO: log the excpetion here

                    return null;
                }
            }
        }

        public void CacheOrUpdate(Uri uri, Stream content)
        {
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
            using (var context = new CacheDataContext(connection))
            {
                try
                {
                    var hash = Utils.ComputeStringMD5Hash(uri.AbsoluteUri, Encoding.UTF8);

                    var cacheIndex = context.GetCacheIndex(hash);

                    if (cacheIndex != null)
                    {
                        var existingCacheFilePath = GetCacheFilePhysicalPath(cacheIndex.Path);

                        if (File.Exists(existingCacheFilePath))
                        {
                            File.Delete(existingCacheFilePath);
                        }

                        // delete history by hash
                        context.DelteCacheIndex(hash);
                    }

                    var newCacheFilePath = GetCacheFilePhysicalPath(hash + ".cache");

                    if (File.Exists(newCacheFilePath))
                    {
                        File.Delete(newCacheFilePath);
                    }

                    using (var fs = new FileStream(newCacheFilePath, FileMode.CreateNew))
                    {
                        content.CopyTo(fs);

                        fs.Flush();
                    }

                    context.InsertCaccheIndex(new CacheIndex
                    {
                        Url = uri.AbsoluteUri,
                        Path = hash + ".cache",
                        CreatedOn = DateTime.Now,
                        ExpiredOn = DateTime.Now.AddHours(24),
                        Hash = hash
                    });
                }
                catch (Exception ex)
                {
                    // TODO: log the excpetion here

                    throw ex;
                }
            }
        }

        private string GetCacheFilePhysicalPath(string path)
        {
            return Path.Combine(_rootFolder, "_cache_", path);
        }

        private void Initialize()
        {
            if(_isInitialized)
            {
                return;
            }

            _sqliteFilePath = Path.Combine(_rootFolder, "index.sqlite3");

            if (!File.Exists(_sqliteFilePath))
            {
                SQLiteConnection.CreateFile(_sqliteFilePath);

                using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", _sqliteFilePath)))
                {
                    var createTableSql = SqlLibrary.Instance.Require("CREATE_INDEX_TABLE");

                    connection.Execute(createTableSql);
                }
            }

            var cacheFolder = Path.Combine(_rootFolder, "_cache_");

            if(!Directory.Exists(cacheFolder))
            {
                Directory.CreateDirectory(cacheFolder);
            }

            // set up SQLite local cache index
            _isInitialized = true;
        }
    }
}
