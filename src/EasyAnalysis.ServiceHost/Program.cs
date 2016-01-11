using EasyAnalysis.Modules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // listen to a task queue

            // execute on task task comming

            // for eaxmple:

            // { handler: 'import-mt-thread', context: { url: '[put-the-url-here]', output: 'landing.threads' } }

            var handlerFactory = new HandlerFactory();

            using (var context = new TaskDbContext())
            {
                while (true)
                {
                    var tasks = context.Tasks.Where(m => m.Status.Equals("NEW")).Take(10).ToList();

                    foreach (var task in tasks)
                    {
                        try
                        {
                            var handler = handlerFactory.Activate(task.Handler);

                            var executionContext = JsonConvert.DeserializeObject<IDictionary<string, object>>(task.Context);

                            handler.OnProcess(executionContext);

                            task.Status = "SUCCESS";
                        }
                        catch (Exception ex)
                        {
                            task.Error = ex.Message;

                            task.Status = "ERROR";

                            Console.WriteLine(ex.Message);
                        }

                        context.SaveChanges();
                    }

                    if(tasks.Count == 0)
                    {
                        System.Threading.Thread.Sleep(10000);
                    }
                }
            }
        }
    }
}
