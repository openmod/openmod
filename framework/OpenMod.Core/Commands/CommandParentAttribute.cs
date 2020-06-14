using System;
using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CommandParentAttribute : Attribute
    {
        public CommandParentAttribute(Type type)
        {
            if (!typeof(ICommand).IsAssignableFrom(type))
            {
                throw new ArgumentException("Type must inherit ICommand", nameof(type));
            }

            CommandType = type;
        }

        public CommandParentAttribute(Type declaringType, string methodName)
        {
            DeclaringType = declaringType;
            MethodName = methodName;
        }

        public Type CommandType { get; }
        public Type DeclaringType { get; }
        public string MethodName { get; }
    }
}