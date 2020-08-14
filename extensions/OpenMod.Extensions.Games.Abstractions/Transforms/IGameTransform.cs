using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Transforms
{
    public interface IGameTransform : IReadOnlyCollection<IGameTransform>
    {
        string TransformName { get; }

        IGameTransform ParentTransform { get; }

        IReadOnlyCollection<IGameTransform> ChildTransforms { get; }

        Vector3 Position { get; }

        Vector3 Velocity { get; }

        Task<bool> SetPositionAsync(Vector3 targetPosition);

        Vector3 Rotation { get; }

        Task<bool> SetRotationAsync(Vector3 rotation);
    }
}