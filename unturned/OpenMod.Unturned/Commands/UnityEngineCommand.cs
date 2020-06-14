using System;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Core.Ioc;
using OpenMod.UnityEngine.Commands;

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