using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    [Service]
    public interface ICurrentCommandContextAccessor
    {
        [CanBeNull]
        ICommandContext Context { get; set; }
    }
}