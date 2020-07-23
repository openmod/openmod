using System;
using OpenMod.API;
using OpenMod.Core.Ioc;
using OpenMod.UnityEngine.Commands;

namespace OpenMod.Unturned.Commands
{
    [DontAutoRegister]
    [OpenModInternal]
    public abstract class UnturnedCommand : UnityEngineCommand
    {
        protected UnturnedCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}