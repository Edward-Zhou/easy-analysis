using EasyAnalysis.Framework.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend
{
    public class DefaultModuleFactory : IModuleFactory
    {
        public IModule CreateInstance(string name)
        {
            return new MSDNMetadataModule() as IModule;
        }
    }
}
