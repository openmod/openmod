using OpenMod.API;
using OpenMod.Core.Ioc;
using OpenMod.UnityEngine.Commands;
using System;

namespace OpenMod.Unturned.Commands
{
    [DontAutoRegister]
    public abstract class UnturnedCommand : UnityEngineCommand
    {
        protected UnturnedCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
