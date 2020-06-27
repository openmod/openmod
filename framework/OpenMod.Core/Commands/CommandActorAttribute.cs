using System;

namespace OpenMod.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CommandActorAttribute : Attribute
    {
        public Type ActorType { get; }

        public CommandActorAttribute(Type actorType)
        {
            ActorType = actorType;
        }
    }
}