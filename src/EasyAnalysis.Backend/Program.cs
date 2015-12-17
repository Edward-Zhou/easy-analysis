using System;
using EasyAnalysis.Framework;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using EasyAnalysis.Infrastructure.Discovery;
using System.Text;

namespace EasyAnalysis.Backend
{
    class Program
    {
        /// <summary>
        /// e.g:
        /// 1) run a init dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:general "parameters:uwp_sort_by_post|init|D:\\forum_cache|landing.threads"
        /// 1) run a monitor dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:general "parameters:uwp_sort_by_lastpost|monitor|D:\\forum_cache|landing.threads"
        /// 2) run a action
        /// EasyAnalysis.Backend.exe type:action name:correct-datatype parameters:landing.threads
        /// </summary>
        /// <param name="args">type:[dataflow|action] name:[e.g. general] parameters:[]</param>
        static void Main(string[] args)
        {
            var text = File.ReadAllText("config.json");

            var keys = JsonConvert.DeserializeObject<Dictionary<string, PaginationDiscoveryConfigration>>(text, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var sb = new StringBuilder();

            foreach(var key in keys)
            {
                sb.AppendLine(
                    string.Format("EasyAnalysis.Backend.exe type:dataflow name:general \"parameters:{0}|init|D:\\forum_cache|landing.sql_threads\"", key.Key));
            }

            var cmd = sb.ToString();

            try
            {
                if (args.Length == 1)
                {
                    var config = File.ReadAllText(args[0]);

                    var sequence = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Options>>(config);

                    foreach (var options in sequence)
                    {
                        Run(options);
                    }

                    return;
                }

                Run(Options.Parse(args));
            }
            catch (Exception ex)
            {
                LogWithInnerException(ex);
            }
        }

        static void LogWithInnerException(Exception ex)
        {
            Logger.Current.Error("Exception: " + ex.Message);

            if (ex.InnerException != null)
            {                
                LogWithInnerException(ex.InnerException);
            }
        }

        static void Run(Options arguments)
        {
            try
            {
                if (arguments.Type == ExecutionType.Dataflow)
                {
                    var exec = new DataflowExec();

                    exec.RunDataFlow(arguments.Parameters);
                }
                else if (arguments.Type == ExecutionType.Action)
                {
                    var factory = new DefaultActionFactory();

                    var action = factory.CreateInstance(arguments.Name);

                    Logger.Current.Info(string.Format("Running action[{0}]", arguments.Name));

                    var task = action.RunAsync(arguments.Parameters);

                    task.Wait();

                    Logger.Current.Info(string.Format("End of running action[{0}]", arguments.Name));
                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);

                throw ex;
            }
        }
    }
}
