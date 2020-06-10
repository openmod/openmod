using System;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : PriorityAttribute
    {
        public string Name { get; }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}