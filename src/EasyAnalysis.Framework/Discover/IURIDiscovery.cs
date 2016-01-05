using System;

namespace EasyAnalysis.Framework
{
    public interface IResourceDiscovery
    {
        event Action<string> OnDiscovered;

        void Start();
    }
}
