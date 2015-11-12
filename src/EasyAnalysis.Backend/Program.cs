﻿using EasyAnalysis.Repository;
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

namespace EasyAnalysis.Backend
{
    class Program
    {
        static void Main(string[] args)
        {
            var steps = new List<Step>();

            //steps.Add(new Step {
            //    Action = "import-new-users",
            //    Parameters = new string[] {
            //        "uwp",
            //        "aug_threads",
            //        "users"
            //    }
            // });

            //steps.Add(new Step
            //{
            //    Action = "extract-user-activies",
            //    Parameters = new string[] {
            //        "uwp",
            //        "aug_threads"
            //    }
            //});

            steps.Add(new Step
            {
                Action = "build-thread-profiles",
                Parameters = new string[] {
                    "uwp", // [repository]
                    "2015-10-1", // [start date]
                    "2015-10-31", // [end date]
                    "uwp_latest", // [thread collection name]
                    "uwp_oct_thread_profiles" // [target collection name]
                }
            });

            var factory = new DefaultActionFactory();

            foreach(var step in steps)
            {
                var action = factory.CreateInstance(step.Action);

                var task = action.RunAsync(step.Parameters);

                task.Wait();
            }
        }
    }
}
