using SDG.Unturned;
using Serilog;

namespace OpenMod.Unturned.Logging
{
    // Serilog is already threaded, no need for ThreadedConsoleInputOutput
    public class SerilogConsoleInputOutput : ConsoleInputOutput
    {
        public override void outputInformation(string information)
        {
            Log.Information(information);
        }

        public override void outputWarning(string warning)
        {
            Log.Warning(warning);
        }

        public override void outputError(string error)
        {
            Log.Error(error);
        }
    }

    public class SerilogWindowsConsoleInputOutput : WindowsConsoleInputOutput
    {
        public override void outputInformation(string information)
        {
            Log.Information(information);
        }

        public override void outputWarning(string warning)
        {
            Log.Warning(warning);
        }

        public override void outputError(string error)
        {
            Log.Error(error);
        }
    }
}