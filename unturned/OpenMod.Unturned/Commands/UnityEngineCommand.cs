using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.UnityEngine.Commands;

namespace OpenMod.Unturned.Commands
{
    [IgnoreCommand]
    public abstract class UnturnedCommand : UnityEngineCommand
    {
        protected UnturnedCommand(ICurrentCommandContextAccessor contextAccessor) : base(contextAccessor)
        {
        }
    }
}