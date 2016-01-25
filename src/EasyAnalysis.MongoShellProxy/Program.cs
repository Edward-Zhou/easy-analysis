using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.MongoShellProxy
{
    class Program
    {
        /// <summary>
        /// mongoshellproxy.exe -file insert_new_records.js -values "repository=uwp&start=2015-1-1&end=2016-1-1"
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // parse parameters
            var parameters = new Dictionary<string, string>();

            int start = 0;

            while (start < args.Length)
            {
                var pair = args.Skip(start).Take(2);

                parameters.Add(pair.ElementAt(0).TrimStart('-'), pair.ElementAt(1));

                start = start + 2;
            }

            // compile js with expressions

            IDictionary<string, string> placeholders = new Dictionary<string, string>();

            var templateJavascript = File.ReadAllText(parameters["file"]);

            var pairs = parameters["values"].Split('&');

            foreach (var pair in pairs)
            {
                var temp = pair.Split('=');

                placeholders.Add(temp[0], temp[1]);
            }

            foreach (var placeholder in placeholders)
            {
                var expression = "{{" + placeholder.Key + "}}";

                var replacement = placeholder.Value;

                templateJavascript = templateJavascript.Replace(expression, replacement);
            }

            var tempPath = Path.GetTempPath();

            var compiledFilePath = Path.Combine(tempPath, Guid.NewGuid().ToString() + "_.js");

            File.WriteAllText(compiledFilePath, templateJavascript);

            // execution

            var applicationExeFile = ConfigurationManager.AppSettings.Get("mongo");

            var arguments = compiledFilePath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal; 

            startInfo.FileName = applicationExeFile;

            startInfo.Arguments = arguments;

            process.StartInfo = startInfo;

            process.Start();

            process.WaitForExit();

            File.Delete(compiledFilePath);
        }
    }
}
