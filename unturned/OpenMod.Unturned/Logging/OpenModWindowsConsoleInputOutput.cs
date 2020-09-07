using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using System;

namespace OpenMod.Unturned.Logging
{
    [OpenModInternal]
    public class OpenModWindowsConsoleInputOutput : OpenModConsoleInputOutput // at the moment they are equal
    {
        public OpenModWindowsConsoleInputOutput(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IAutoCompleteHandler autoCompleteHandler) : base(loggerFactory, configuration, autoCompleteHandler)
        {
        }
    }
}