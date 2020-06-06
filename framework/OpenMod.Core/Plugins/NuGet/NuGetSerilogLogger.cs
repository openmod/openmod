using System;
using System.Threading.Tasks;
using NuGet.Common;

namespace OpenMod.Core.Plugins.NuGet
{
    public class NuGetSerilogLogger : LoggerBase
    {
        public override void Log(ILogMessage message)
        {
            var level = message.Level;
            string prefix = "[NuGet] ";
            var text = prefix + message.Message;

            switch (level)
            {
                case LogLevel.Debug:
                    Serilog.Log.Debug(text);
                    break;
                case LogLevel.Verbose:
                    Serilog.Log.Debug(text);
                    break;
                case LogLevel.Information:
                    Serilog.Log.Information(text);
                    break;
                case LogLevel.Minimal:
                    Serilog.Log.Information(text);
                    break;
                case LogLevel.Warning:
                    Serilog.Log.Warning(text);
                    break;
                case LogLevel.Error:
                    Serilog.Log.Error(text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}