using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Buildings
{
    public interface IBuildable
    {
        IBuildableAsset Asset { get; }

        IWorldTransform Transform { get; }

        IBuildableState State { get; }

        IOwnership Ownership { get; }

        string BuildableInstanceId { get; }

        Task DestroyAsync();
    }
}