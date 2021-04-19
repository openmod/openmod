using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Transforms
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWorldTransform : IReadOnlyCollection<IWorldTransform>
    {
        /// <summary>
        /// Gets the name of the transform.
        /// </summary>
        string? TransformName { get; }

        /// <summary>
        /// Gets the parent transform of this transform.
        /// </summary>
        IWorldTransform? ParentTransform { get; }

        /// <summary>
        /// Gets the child transforms of this transform.
        /// </summary>
        IReadOnlyCollection<IWorldTransform> ChildTransforms { get; }

        /// <summary>
        /// Gets the position of the transform.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Gets the velocity of the transform.
        /// </summary>
        Vector3 Velocity { get; }

        /// <summary>
        /// Gets the rotation of the transform.
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// Sets the velocity of the transform.
        /// </summary>
        /// <param name="velocity">The velocity to set.</param>
        /// <exception cref="NotSupportedException">Thrown if this transform does not support velocity.</exception>
        /// <returns><b>True</b> if successful; otherwise, <b>false</b>.</returns>
        Task<bool> SetVelocityAsync(Vector3 velocity);

        /// <summary>
        /// Sets the position of the transform.
        /// </summary>
        /// <param name="position">The position to set to.</param>
        /// <returns><b>True</b> if successful; otherwise, <b>false</b>.</returns>
        Task<bool> SetPositionAsync(Vector3 position);

        /// <summary>
        /// Sets the rotation of the transform.
        /// </summary>
        /// <param name="rotation">The rotation to set to.</param>
        /// <returns><b>True</b> if successful; otherwise, <b>false</b>.</returns>
        Task<bool> SetRotationAsync(Quaternion rotation);

        /// <summary>
        /// Destorys the transform.
        /// </summary>
        Task DestroyAsync();
    }
}