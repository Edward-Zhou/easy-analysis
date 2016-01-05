using EasyAnalysis.Framework.Analysis;

namespace EasyAnalysis.Modules
{
    public class DefaultModuleFactory : IMetadataProcessModuleFactory
    {
        public IMetadataProcessModule Activate(string name)
        {
            return new MSDNMetadataModule() as IMetadataProcessModule;
        }
    }
}
