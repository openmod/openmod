using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Core.Ioc;

namespace OpenMod.UnityEngine.Commands
{
    [DontAutoRegister]
    public abstract class UnityEngineCommand : CommandBase
    {
        protected UnityEngineCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public sealed override async Task ExecuteAsync()
        {
            await OnExecuteAsync();
        }

        protected abstract UniTask OnExecuteAsync();
    }
}