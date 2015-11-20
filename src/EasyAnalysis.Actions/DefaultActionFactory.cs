using EasyAnalysis.Actions;
using EasyAnalysis.Framework.Analysis;
using System;

namespace EasyAnalysis.Backend
{
    public class DefaultActionFactory : IActionFactory
    {
        public IAction CreateInstance(string name)
        {
            switch(name.ToLower())
            {
                case "build-basic-threadprofiles":
                    return new BuildBasicThreadProfiles(new UniversalConnectionStringProvider());

                case "extract-user-activies":
                    return new ExtractUserActivies(new UniversalConnectionStringProvider());

                case "import-new-users":
                    return new ImportNewUsers(new UniversalConnectionStringProvider());

                case "clean-up-data":
                    return new CleanUpData();

                case "add-metadata-to-threadprofile":
                    return new AddMetadataToThreadProfile(new UniversalConnectionStringProvider());

                case "sync-with-web-database":
                    return new SyncWithWebDatabase();

                default: throw new NotImplementedException(string.Format("Action[{0}] is not supported yet"));
            }
        }
    }
}
