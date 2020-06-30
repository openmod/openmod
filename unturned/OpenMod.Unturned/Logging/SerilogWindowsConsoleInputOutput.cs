using Microsoft.Extensions.Logging;

namespace OpenMod.Unturned.Logging
{
    public class SerilogWindowsConsoleInputOutput : SerilogConsoleInputOutput // at the moment they are equal
    {
        public SerilogWindowsConsoleInputOutput(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }
    }
}