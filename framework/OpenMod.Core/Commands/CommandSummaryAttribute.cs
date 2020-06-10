using System;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandSummaryAttribute : Attribute
    {
        public string Summary { get; }

        public CommandSummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}