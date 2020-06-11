using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;

namespace OpenMod.UnityEngine.Commands
{
    [IgnoreCommand]
    public abstract class UnityEngineCommand : CommandBase
    {
        protected UnityEngineCommand(ICurrentCommandContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        public sealed override async Task ExecuteAsync()
        {
            await OnExecuteAsync();
        }

        protected abstract UniTask OnExecuteAsync();
    }
}