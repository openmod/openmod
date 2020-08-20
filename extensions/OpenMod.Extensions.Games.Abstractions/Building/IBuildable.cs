using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    public interface IBuildable : IHasOwnership
    {
        IBuildableAsset Asset { get; }

        IWorldTransform Transform { get; }

        IBuildableState State { get; }

        string BuildableInstanceId { get; }

        Task DestroyAsync();
    }
}