using EasyAnalysis.Framework.Analysis;

namespace EasyAnalysis.Modules
{
    public class DefaultModuleFactory : IModuleFactory
    {
        public IModule CreateInstance(string name)
        {
            return new MSDNMetadataModule() as IModule;
        }
    }
}
