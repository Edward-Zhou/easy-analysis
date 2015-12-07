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
        private static System.Timers.Timer aTimer;
        static void Main(string[] args)
        {
            try
            {
#if DEBUG
                SetTimer(1000);
#else
                SetTimer(7200000);
#endif

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
            ExeCMD(@"D:\GitHubVisualStudio\easy-analysis\src\EasyAnalysis.Backend\bin\Release\EasyAnalysis.Backend.exe", @"D:\temp\sovso_import_actions.json", System.Diagnostics.ProcessWindowStyle.Normal);
            ExeCMD(@"D:\GitHubVisualStudio\easy-analysis\src\EasyAnalysis.Backend\bin\Release\EasyAnalysis.Backend.exe", @"D:\temp\sotfs_import_actions.json", System.Diagnostics.ProcessWindowStyle.Normal);
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
