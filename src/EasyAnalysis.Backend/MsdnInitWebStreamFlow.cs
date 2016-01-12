using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Config;
using EasyAnalysis.Infrastructure.Discovery;

namespace EasyAnalysis.Backend
{
    public class MsdnInitWebStreamFlow : IWebStreamFlow
    {
        public void Run(string[] parameters)
        {
            string settingName = parameters[0].ToLower();

            string outputCollectionName = parameters[1];

            IResourceDiscovery discovery = CreateDiscoveryBySettingName(settingName);

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

        private static IResourceDiscovery CreateDiscoveryBySettingName(string settingName)
        {
            var jsonConfigrationManager = new JsonConfigrationManager<PaginationDiscoveryConfigration>("config.json");

            var paginationDiscoveryConfigration = jsonConfigrationManager.GetSetting(settingName);

            if (paginationDiscoveryConfigration == null)
            {
                throw new System.ArgumentException(string.Format("setting [{0}] not found", settingName));
            }

            Logger.Current.Info("Run general dataflow in init mode");

            IResourceDiscovery discovery = new PaginationDiscovery(paginationDiscoveryConfigration);

            return discovery;
        }
    }
}
