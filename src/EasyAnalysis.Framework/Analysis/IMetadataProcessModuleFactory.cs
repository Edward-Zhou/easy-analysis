namespace EasyAnalysis.Framework.Analysis
{
    public interface IMetadataProcessModuleFactory
    {
        IMetadataProcessModule Activate(string name);
    }
}
