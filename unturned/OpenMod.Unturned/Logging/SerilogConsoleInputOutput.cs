using Microsoft.Extensions.Logging;
using SDG.Unturned;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OpenMod.Unturned.Logging
{
    public class SerilogConsoleInputOutput : ConsoleInputOutput
    {
        private readonly ILogger m_Logger;

        public SerilogConsoleInputOutput(ILoggerFactory loggerFactory)
        {
            m_Logger = loggerFactory.CreateLogger("SDG.Unturned");
        }

        public override void outputInformation(string information)
        {
            m_Logger.LogInformation(information);
            System.Console.CursorTop++;
            System.Console.CursorLeft = 0;
        }

        public override void outputWarning(string warning)
        {
            m_Logger.LogWarning(warning);
            System.Console.CursorTop++;
            System.Console.CursorLeft = 0;
        }

        public override void outputError(string error)
        {
            m_Logger.LogError(error);
            System.Console.CursorTop++;
            System.Console.CursorLeft = 0;
        }
    }
}