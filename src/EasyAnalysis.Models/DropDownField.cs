using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Models
{
    public class Option
    {
        public Int32 Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class DropDownField
    {
        public Int32 Id { get; set; }

        public string Repository { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("options")]
        public ICollection<Option> Options { get; set; }

        public DropDownField()
        {
            Options = new List<Option>();
        }
    }
}