using EasyAnalysis.Actions;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
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

                default: throw new NotImplementedException(string.Format("Action[{0}] is not supported yet"));
            }
        }
    }
}
