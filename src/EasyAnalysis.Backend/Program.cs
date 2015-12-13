using System;
using EasyAnalysis.Framework;
using System.IO;
using System.Collections.Generic;

namespace EasyAnalysis.Backend
{
    class Program
    {
        /// <summary>
        /// e.g:
        /// 1) run a init dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:general "parameters:init:1&10|D:\\forum_cache|landing.threads"
        /// 1) run a monitor dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:general "parameters:monitor|D:\\forum_cache|landing.threads"
        /// 2) run a action
        /// EasyAnalysis.Backend.exe type:action name:correct-datatype parameters:landing.threads
        /// </summary>
        /// <param name="args">type:[dataflow|action] name:[e.g. general] parameters:[]</param>
        static void Main(string[] args)
        {
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
                Logger.Current.Error(ex.Message);

                if(ex.InnerException != null)
                {
                    Logger.Current.Error("Inner Exception: " + ex.Message);
                }
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
