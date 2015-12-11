using EasyAnalysis.Actions;
using EasyAnalysis.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EasyAnalysis.ScheduledTask
{
    class Program
    {
        private static string filePath;
        private static System.Timers.Timer aTimer;
        private static List<string> jsonList = new List<string>();
        static void Main(string[] args)
        {
            try
            {
                if (args == null | args.Length <= 2)//At lease three parameters
                {
                    Console.WriteLine("Please enter the correct parameters"+Environment.NewLine+ "[Backend executable file path] [Refresh interval(seconds)] [json file1 path] [json file2 path]...");
                }
                else
                {
                    filePath = args[0];
                    if(!File.Exists(filePath))
                    {
                        throw new Exception("The Backend file can not be found");
                    }
                    Int32 refreshInterval = Convert.ToInt32(args[1]) * 1000;
                    
                    for (int i = 2; i < args.Length; i++) // Loop through args
                    {
                        if (File.Exists(args[i]))
                        {
                            jsonList.Add(args[i]);
                        }
                        else
                        {
                            throw new Exception("The Backend file can not be found");
                        }
                    }

#if DEBUG
                    SetTimer(1000);
#else
                    SetTimer(refreshInterval);
#endif
                }


                Console.WriteLine("\nPress the Enter key to exit the application...\n");
                Logger.Current.Info(string.Format("The ScheduledTask started at {0:HH:mm:ss.fff}", DateTime.Now));
                Console.ReadLine();
                aTimer.Stop();
                aTimer.Dispose();

                Console.WriteLine("Terminating the ScheduledTask...");

            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);
            }
        }
        private static void SetTimer(Int32 interval)
        {
            aTimer = new System.Timers.Timer(interval);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Logger.Current.Info(string.Format("The ScheduledTask Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime));

            foreach (var json in jsonList)
            {
#if DEBUG
                var pws = System.Diagnostics.ProcessWindowStyle.Normal;
#else
                var pws = System.Diagnostics.ProcessWindowStyle.Hidden;
#endif
                ExeCMD(filePath, json, pws);
            }
        }

        /// <summary>
        /// Execuate command
        /// </summary>
        /// <param name="fileName">FileName for ProcessStartInfo</param>
        /// <param name="configPath">Arguments for ProcessStartInfo</param>
        private static void ExeCMD(string fileName, string configPath, System.Diagnostics.ProcessWindowStyle pws)
        {
            UpdateJSONConfig(configPath);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = pws;
            startInfo.FileName = fileName;
            startInfo.Arguments = configPath;
            process.StartInfo = startInfo;
            process.Start();
            Console.WriteLine("The command execuated at {0:HH:mm:ss.fff}", DateTime.Now);
#if DEBUG
            aTimer.Stop();
#endif
        }

        private static void UpdateJSONConfig(string configPath)
        {
            var config = File.ReadAllText(configPath);
            var sequence = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Options>>(config);

            foreach (var options in sequence)
            {
                Update(options);
            }

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(sequence, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(configPath, output);
        }

        private static void Update(Options arguments)
        {
            try
            {
                DateTime dtUtcNow = DateTime.UtcNow;

                for(int i=0; i< arguments.Parameters.Count(); i++)
                {
                    var para = arguments.Parameters[i];
                    if (para.ToString().Split('&').Count() == 2)
                    {
                        var timeFrameRange = TimeFrameRange.Parse(para);
                        timeFrameRange.Start = dtUtcNow.AddDays(-2); //-2 days
                        timeFrameRange.End = dtUtcNow.AddDays(2); //+2 days
                        arguments.Parameters[i] = TimeFrameRange.ParseBack(timeFrameRange);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);
                //throw ex;
            }
        }

    }
}
