using System;
using System.Threading.Tasks;
using NuGet.Common;

namespace OpenMod.NuGet
{
    public class NuGetConsoleLogger : LoggerBase
    {
        public override void Log(ILogMessage message)
        {
            if (message.Level < LogLevel.Information)
            {
                return;
            }

            if (message.Message.Contains("Resolving dependency information took"))
            {
                return;
            }

            if (message.Message.Trim().StartsWith("CACHE"))
            {
                return;
            }

            Console.WriteLine($"[{message.Level}] [NuGet] {message.Message}");
        }

        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}