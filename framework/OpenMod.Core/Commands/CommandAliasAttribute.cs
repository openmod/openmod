using System;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAliasAttribute :Attribute
    {
        public string Alias { get; }

        public CommandAliasAttribute(string alias)
        {
            Alias = alias;
        }
    }
}