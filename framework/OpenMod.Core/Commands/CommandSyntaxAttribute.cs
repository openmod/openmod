using System;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandSyntaxAttribute : Attribute
    {
        public string Syntax { get; }

        public CommandSyntaxAttribute(string syntax)
        {
            Syntax = syntax;
        }
    }
}