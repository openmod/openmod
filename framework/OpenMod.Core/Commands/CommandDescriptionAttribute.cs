using System;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public CommandDescriptionAttribute(string description)
        {
            Description= description;
        }
    }
}