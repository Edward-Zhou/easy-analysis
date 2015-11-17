﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend
{
    public enum ExecutionType
    {
        Unkown = 0,
        Dataflow = 1,
        Action = 2
    }

    public class Arguments
    {
        public ExecutionType Type { get; set; }

        public string Name { get; set; }

        public string[] Parameters { get; set; }

        public static Arguments Parse(string[] args)
        {
            if(args.Length != 3)
            {
                throw new ArgumentException(Messages.ARG_ERROR);
            }

            var dict = new Dictionary<string, string>();

            foreach(var arg in args)
            {
                var indexOfFirstCharacter = arg.IndexOf(':');

                var subArgName = arg.Substring(0, indexOfFirstCharacter);

                var subArgValue = arg.Substring(indexOfFirstCharacter + 1);

                dict.Add(subArgName, subArgValue);
            }

            try
            {
                var arguments = new Arguments();

                var type = dict["type"].ToLower();

                switch(type)
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

                if(arguments.Type == ExecutionType.Unkown)
                {
                    throw new ArgumentException(Messages.ARG_ERROR);
                }

                var name = dict["name"];

                if(string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException(Messages.ARG_ERROR);
                }

                arguments.Name = name;

                var temp = dict["parameters"];

                if (string.IsNullOrWhiteSpace(temp))
                {
                    throw new ArgumentException(Messages.ARG_ERROR);
                }

                var parameters = temp.Split('|');

                arguments.Parameters = parameters;

                return arguments;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(Messages.ARG_ERROR);
            }
        }
    }
}
