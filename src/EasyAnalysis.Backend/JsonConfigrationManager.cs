using EasyAnalysis.Infrastructure.Discovery;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend
{
    internal class JsonConfigrationManager
    {
        private IDictionary<string, PaginationDiscoveryConfigration> _settings;

        private JsonConfigrationManager()
        {
            var text = File.ReadAllText("config.json");

            _settings = JsonConvert.DeserializeObject<Dictionary<string, PaginationDiscoveryConfigration>>(text, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private static JsonConfigrationManager _current = new JsonConfigrationManager();

        public static JsonConfigrationManager Current
        {
            get { return _current; }
        }

        public PaginationDiscoveryConfigration GetSetting(string name)
        {
            if(_settings.ContainsKey(name))
            {
                return _settings[name];
            }
            else
            {
                return null;
            }
        }
    }
}
