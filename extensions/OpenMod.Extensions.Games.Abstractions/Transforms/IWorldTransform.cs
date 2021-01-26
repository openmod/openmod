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
        /// <value>
        /// The name of the transform. Can be null.
        /// </value>
        string? TransformName { get; }

        /// <value>
        /// The parent transform of this transform. Can be null.
        /// </value>
        IWorldTransform? ParentTransform { get; }

        /// <value>
        /// The child transforms of this transform.
        /// </value>
        IReadOnlyCollection<IWorldTransform> ChildTransforms { get; }

        /// <value>
        /// The position of the transform.
        /// </value>
        Vector3 Position { get; }

        /// <value>
        /// The velocity of the transform.
        /// </value>
        Vector3 Velocity { get; }

        /// <value>
        /// The rotation in euler angles of the transform.
        /// </value>
        Vector3 Rotation { get; }

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
        Task<bool> SetRotationAsync(Vector3 rotation);

        /// <summary>
        /// Destorys the transform.
        /// </summary>
        Task DestroyAsync();
    }
}