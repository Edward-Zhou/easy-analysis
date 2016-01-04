using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;

namespace EasyAnalysis.Backend
{
    internal class JsonConfigrationManager<TNode>
    {
        private IDictionary<string, TNode> _settings;

        public JsonConfigrationManager(string file)
        {
            var text = File.ReadAllText(file);

            _settings = JsonConvert.DeserializeObject<Dictionary<string, TNode>>(text, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public TNode GetSetting(string name)
        {
            if(_settings.ContainsKey(name))
            {
                return _settings[name];
            }
            else
            {
                return default(TNode);
            }
        }
    }
}
