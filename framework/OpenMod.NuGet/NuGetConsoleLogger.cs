using System;
using System.Threading.Tasks;
using NuGet.Common;

namespace OpenMod.NuGet
{
    public class NuGetConsoleLogger : LoggerBase
    {
        private readonly LogLevel m_MinimalLogLevel;

        public NuGetConsoleLogger() : base(LogLevel.Information)
        {
        }

        public NuGetConsoleLogger(LogLevel minimalLogLevel)
        {
            m_MinimalLogLevel = minimalLogLevel;
        }

        public override void Log(ILogMessage message)
        {
            if (message.Level < m_MinimalLogLevel)
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