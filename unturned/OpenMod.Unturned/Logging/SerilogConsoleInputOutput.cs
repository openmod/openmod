using Microsoft.Extensions.Logging;
using SDG.Unturned;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OpenMod.Unturned.Logging
{
    // Serilog is already threaded, no need to make it ThreadedConsoleInputOutput
    public class SerilogConsoleInputOutput : ThreadedConsoleInputOutput
    {
        private readonly ILogger m_Logger;

        public SerilogConsoleInputOutput(ILoggerFactory loggerFactory)
        {
            m_Logger = loggerFactory.CreateLogger("SDG.Unturned");
        }

        public override void outputInformation(string information)
        {
            m_Logger.LogInformation(information);
        }

        public override void outputWarning(string warning)
        {
            m_Logger.LogWarning(warning);
        }

        public override void outputError(string error)
        {
            m_Logger.LogError(error);
        }
    }

    public class SerilogWindowsConsoleInputOutput : ThreadedWindowsConsoleInputOutput
    {
        private readonly ILogger m_Logger;

        public SerilogWindowsConsoleInputOutput(ILoggerFactory loggerFactory)
        {
            m_Logger = loggerFactory.CreateLogger("SDG.Unturned");
        }

        public override void outputInformation(string information)
        {
            m_Logger.LogInformation(information);
        }

        public override void outputWarning(string warning)
        {
            m_Logger.LogWarning(warning);
        }

        public override void outputError(string error)
        {
            m_Logger.LogError(error);
        }
    }
}