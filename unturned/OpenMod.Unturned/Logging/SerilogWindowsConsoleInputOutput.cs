using Microsoft.Extensions.Logging;
using SDG.Unturned;

namespace OpenMod.Unturned.Logging
{
    public class SerilogWindowsConsoleInputOutput : WindowsConsoleInputOutput
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