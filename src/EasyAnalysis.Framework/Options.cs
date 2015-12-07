using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework
{
    public enum ExecutionType
    {
        Unkown = 0,
        Dataflow = 1,
        Action = 2
    }

    public class Options
    {
        [JsonProperty("type")]
        public ExecutionType Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parameters")]
        public string[] Parameters { get; set; }

        public static Options Parse(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("The arguments are incorrect");
            }

            var dict = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var indexOfFirstCharacter = arg.IndexOf(':');

                var subArgName = arg.Substring(0, indexOfFirstCharacter);

                var subArgValue = arg.Substring(indexOfFirstCharacter + 1);

                dict.Add(subArgName, subArgValue);
            }

            try
            {
                var arguments = new Options();

                var type = dict["type"].ToLower();

                switch (type)
                {
                    case "dataflow":
                        arguments.Type = ExecutionType.Dataflow;
                        break;
                    case "action":
                        arguments.Type = ExecutionType.Action;
                        break;
                    default:
                        arguments.Type = ExecutionType.Unkown;
                        break;
                }

                if (arguments.Type == ExecutionType.Unkown)
                {
                    throw new ArgumentException("The arguments are incorrect");
                }

                var name = dict["name"];

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("The arguments are incorrect");
                }

                arguments.Name = name;

                if (!dict.ContainsKey("parameters"))
                {
                    return arguments;
                }

                var temp = dict["parameters"];

                if (string.IsNullOrWhiteSpace(temp))
                {
                    throw new ArgumentException("The arguments are incorrect");
                }

                var parameters = temp.Split('|');

                arguments.Parameters = parameters;

                return arguments;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The arguments are incorrect");
            }
        }
    }
}
