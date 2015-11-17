using EasyAnalysis.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework;
using EasyAnalysis.Infrastructure.Discovery;
using EasyAnalysis.Infrastructure.Cache;
using EasyAnalysis.Modules;
using EasyAnalysis.Infrastructure.IO;

namespace EasyAnalysis.Backend
{
    class Program
    {
        /// <summary>
        /// e.g:
        /// 1) run a monitor dataflow 
        /// EasyAnalysis.Backend.exe type:dataflow name:general parameters:init|D:\forum_cache|landing.test
        /// 2) run a action
        /// EasyAnalysis.Backend.exe type:action name:correct-datatype parameters:landing.threads
        /// </summary>
        /// <param name="args">type:[dataflow|action] name:[e.g. general] parameters:[]</param>
        static void Main(string[] args)
        {
            var arguments = Arguments.Parse(args);

            if (arguments.Type == ExecutionType.Dataflow)
            {
                var exec = new DataflowExec();

                exec.RunDataFlow(arguments.Parameters);
            }
            else if (arguments.Type == ExecutionType.Action)
            {
                var factory = new DefaultActionFactory();

                var action = factory.CreateInstance(arguments.Name);

                var task = action.RunAsync(arguments.Parameters);

                task.Wait();
            }
        }
    }
}
