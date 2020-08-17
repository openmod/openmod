using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Transforms
{
    public interface IWorldTransform : IReadOnlyCollection<IWorldTransform>
    {
        string TransformName { get; }

        IWorldTransform ParentTransform { get; }

        IReadOnlyCollection<IWorldTransform> ChildTransforms { get; }

        Vector3 Position { get; }

        Vector3 Velocity { get; }

        Task<bool> SetVelocityAsync(Vector3 velocity);
        
        Task<bool> SetPositionAsync(Vector3 targetPosition);

        Vector3 Rotation { get; }

        Task<bool> SetRotationAsync(Vector3 rotation);

        Task DestroyAsync();
    }
}