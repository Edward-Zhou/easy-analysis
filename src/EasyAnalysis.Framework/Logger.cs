using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework
{
    public class Logger
    {
        static Logger()
        {
            SetupLog4Net();
        }

        private static readonly ILog InternalLogger = LogManager.GetLogger("eas.log");

        public static Logger Current = new Logger();

        public void Info(string info)
        {
            InternalLogger.Info(info);
        }

        public void Error(string err)
        {
            InternalLogger.Error(err);
        }

        public void Warn(string warnning)
        {
            InternalLogger.Warn(warnning);
        }

        private static void SetupLog4Net()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };

            patternLayout.ActivateOptions();

            RollingFileAppender rollingFileAppender = new RollingFileAppender
            {
                AppendToFile = false,
                File = @"logs\",
                Layout = patternLayout,
                MaxSizeRollBackups = 5,
                DatePattern = "yyyyMMddHHmm'.log'",
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Composite,
                StaticLogFileName = false
            };

            ColoredConsoleAppender coloredConsoleAppender = new ColoredConsoleAppender
            {
                Layout = patternLayout
            };

            coloredConsoleAppender
                .AddMapping(
                new ColoredConsoleAppender.LevelColors {
                 Level = Level.Warn,
                 ForeColor = ColoredConsoleAppender.Colors.Yellow
            });

            coloredConsoleAppender
                .AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Error,
                ForeColor = ColoredConsoleAppender.Colors.Red
            });

            rollingFileAppender.ActivateOptions();

            coloredConsoleAppender.ActivateOptions();

            hierarchy.Root.AddAppender(rollingFileAppender);

            hierarchy.Root.AddAppender(coloredConsoleAppender);

            hierarchy.Root.Level = Level.Info;

            hierarchy.Configured = true;
        }
    }
}
