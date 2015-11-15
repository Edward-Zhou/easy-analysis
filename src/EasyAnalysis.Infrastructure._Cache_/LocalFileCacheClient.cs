using EasyAnalysis.Framework.Cache;
using System;
using System.IO;

namespace EasyAnalysis.Infrastructure.Cache
{
    public class LocalFileCacheClient : ICacheClient
    {
        private readonly LocalFileCacheServcie _service;

        internal LocalFileCacheClient(LocalFileCacheServcie service)
        {
            _service = service;
        }

        public CacheStatus GetStatus(Uri resouces)
        {
            return _service.GetStatus(resouces);
        }

        Stream ICacheClient.GetCache(Uri resource)
        {
            return _service.GetCache(resource);
        }

        void ICacheClient.SetCache(Uri resource, Stream stream)
        {
            _service.CacheOrUpdate(resource, stream);
        }
    }
}
