using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Rust.Transforms
{
    public class RustNetworkableTransform : IWorldTransform
    {
        private readonly BaseNetworkable m_BaseNetworkable;
        private readonly UnityTransform m_UnityTransform;

        public RustNetworkableTransform(BaseNetworkable baseNetworkable)
        {
            m_BaseNetworkable = baseNetworkable;
            m_UnityTransform = new UnityTransform(baseNetworkable.transform);
        }

        public IEnumerator<IWorldTransform> GetEnumerator()
        {
            return ChildTransforms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return m_BaseNetworkable.children.Count; }
        }

        public string? TransformName
        {
            get { return m_BaseNetworkable.name; }
        }

        public IWorldTransform? ParentTransform
        {
            get
            {
                return m_BaseNetworkable.HasParent()
                    ? new RustNetworkableTransform(m_BaseNetworkable.GetParentEntity())
                    : null;
            }
        }

        public IReadOnlyCollection<IWorldTransform> ChildTransforms
        {
            get
            {
                return m_BaseNetworkable.children
                    .Select(x => new RustNetworkableTransform(x))
                    .ToList();
            }
        }

        public Vector3 Position
        {
            get { return m_UnityTransform.Position; }
        }

        public Vector3 Velocity
        {
            get { return m_UnityTransform.Velocity; }
        }

        public Task<bool> SetVelocityAsync(Vector3 velocity)
        {
            return m_UnityTransform.SetVelocityAsync(velocity);
        }

        public Task<bool> SetPositionAsync(Vector3 targetPosition)
        {
            return m_UnityTransform.SetPositionAsync(targetPosition);
        }

        public Quaternion Rotation
        {
            get { return m_UnityTransform.Rotation; }
        }

        public Task<bool> SetRotationAsync(Quaternion rotation)
        {
            return m_UnityTransform.SetRotationAsync(rotation);
        }

        public Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                if (!m_BaseNetworkable.IsDestroyed)
                {
                    m_BaseNetworkable.Kill();
                }
            }

            return DestroyTask().AsTask();
        }
    }
}
