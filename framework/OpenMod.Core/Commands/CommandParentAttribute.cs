using System;
using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CommandParentAttribute : Attribute
    {
        public CommandParentAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(ICommand).IsAssignableFrom(type))
            {
                throw new ArgumentException("Type must inherit ICommand", nameof(type));
            }

            CommandType = type;
        }

        public CommandParentAttribute(Type declaringType, string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException(nameof(methodName));
            }

            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            MethodName = methodName;
        }

        public Type? CommandType { get; }
        public Type? DeclaringType { get; }
        public string? MethodName { get; }
    }
}