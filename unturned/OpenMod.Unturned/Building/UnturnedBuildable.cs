using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Building
{
    public abstract class UnturnedBuildable : IBuildable
    {
        public IBuildableAsset Asset { get; }

        public IWorldTransform Transform { get; }

        public IBuildableState State { get; }

        public IOwnership Ownership { get; }

        public string BuildableInstanceId { get; }

        protected UnturnedBuildable(IBuildableAsset asset, IWorldTransform transform, IBuildableState state,
            IOwnership ownership, string buildableInstanceId)
        {
            Asset = asset;
            Transform = transform;
            State = state;
            Ownership = ownership;
            BuildableInstanceId = buildableInstanceId;
        }

        public abstract Task DestroyAsync();
    }
}
