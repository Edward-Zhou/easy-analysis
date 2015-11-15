using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Cache
{
    public enum CacheStatus
    {
        None = 0,
        Active = 1,
        Expired = 2,
        Deleted = 3
    }

    public interface ICacheClient
    {
        Stream GetCache(Uri resource);

        void SetCache(Uri resource, Stream stream);

        CacheStatus GetStatus(Uri resouces);
    }
}
