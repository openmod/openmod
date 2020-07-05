using System;
using Microsoft.Extensions.Logging;

namespace OpenMod.Unturned.Logging
{
    public class OpenModWindowsConsoleInputOutput : OpenModConsoleInputOutput // at the moment they are equal
    {
        public OpenModWindowsConsoleInputOutput(
            ILoggerFactory loggerFactory,
            IAutoCompleteHandler autoCompleteHandler) : base(loggerFactory, autoCompleteHandler)
        {
        }
    }
}