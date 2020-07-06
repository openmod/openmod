using System;
using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public class CommandParameterParseException : UserFriendlyException
    {
        public string Argument { get; }
        public Type ExpectedType { get; }

        public CommandParameterParseException(string message, string argument, Type expectedType) : base(message)
        {
            Argument = argument;
            ExpectedType = expectedType;
        }
    }
}