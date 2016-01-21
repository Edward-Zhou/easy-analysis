using System;
using EasyAnalysis.Framework;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using EasyAnalysis.Infrastructure.Discovery;
using System.Text;
using System.Linq;

namespace EasyAnalysis.Backend
{
    class Program
    {
        /// <summary>
        /// e.g:
        /// 1) run a init dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:msdn-monitor "parameters:uwp_sort_by_post|D:\\forum_cache|landing.threads"
        /// 2) run a monitor dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:msdn-init "parameters:uwp_sort_by_lastpost|D:\\forum_cache|landing.threads"
        /// 3) run a refresh dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:msdn-refresh "parameters:D:\\forum_cache|uwp.thread_profiles|landing.threads|2015-12-01T00:00:00Z&2015-12-20T00:00:00Z"
        /// 4) run a action
        /// EasyAnalysis.Backend.exe type:action name:correct-datatype parameters:landing.threads
        /// 5) run a package
        /// EasyAnalysis.Backend.exe type:package name:{package_name} parameters:start=value1&end=value2
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
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
                    var name = arguments.Name.ToLower();

                    var webStreamFlowFactory = new DefaultWebStreamFlowFactory();

                    var webStreamFlow = webStreamFlowFactory.Activate(name);

                    webStreamFlow.Run(arguments.Parameters);
                }
                else if (arguments.Type == ExecutionType.Action)
                {
                    var actionFactory = new DefaultActionFactory();

                    var action = actionFactory.Activate(arguments.Name);

                    Logger.Current.Info(string.Format("Running action[{0}]", arguments.Name));

                    var task = action.RunAsync(arguments.Parameters);

                    task.Wait();

                    Logger.Current.Info(string.Format("End of running action[{0}]", arguments.Name));
                }
                else if (arguments.Type == ExecutionType.Package)
                {
                    var text = File.ReadAllText(arguments.Name);

                    if(arguments.Parameters != null && arguments.Parameters.Length > 0)
                    {
                        IDictionary<string, string> placeholders = new Dictionary<string, string>();

                        var firstParameter = arguments.Parameters.FirstOrDefault();

                        var pairs = firstParameter.Split('&');

                        foreach(var pair in pairs)
                        {
                            var temp = pair.Split('=');

                            placeholders.Add(temp[0], temp[1]);
                        }

                        foreach (var placeholder in placeholders)
                        {
                            var expression = "{{" + placeholder.Key + "}}";

                            var replacement = placeholder.Value;

                            text = text.Replace(expression, replacement);
                        }

                        var sequence = JsonConvert.DeserializeObject<List<Options>>(text);

                        foreach (var options in sequence)
                        {
                            Run(options);
                        }
                    }
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
