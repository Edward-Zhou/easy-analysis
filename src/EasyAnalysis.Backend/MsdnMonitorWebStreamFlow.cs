using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Config;
using EasyAnalysis.Infrastructure.Discovery;
using System;

namespace EasyAnalysis.Backend
{
    public class MsdnMonitorWebStreamFlow : IWebStreamFlow
    {
        public void Run(string[] parameters)
        {
            string settingName = parameters[0].ToLower();

            string outputCollectionName = parameters[1];

            var jsonConfigrationManager = new JsonConfigrationManager<PaginationDiscoveryConfigration>("config.json");

            var paginationDiscoveryConfigration = jsonConfigrationManager.GetSetting(settingName);

            if (paginationDiscoveryConfigration == null)
            {
                throw new ArgumentException(string.Format("setting [{0}] not found", settingName));
            }

            IResourceDiscovery discovery = new PeriodicPaginationDiscovery(paginationDiscoveryConfigration);

            using (var client = new Message.MessageClient("import-new-question"))
            {
                discovery.OnDiscovered += (url) =>
                {
                    var cmd = new Message.Command.ImportQuestionCommand
                    {
                        Url = url,
                        Collection = outputCollectionName
                    };

                    client.Send(cmd);

                    Logger.Current.Info(string.Format("Discovered [{0}]", url));
                };

                discovery.Start();
            }

        }
    }
}
