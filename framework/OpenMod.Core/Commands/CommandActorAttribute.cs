using System;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandActorAttribute : Attribute
    {
        public Type[] CommandActorTypes { get; set; }
    }
}