using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Extensions;
using OpenMod.UnityEngine.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.UnityEngine.Transforms
{
    public class UnityTransform : IWorldTransform
    {
        private readonly Transform m_Transform;
        private readonly Rigidbody? m_Rigidbody;

        public UnityTransform(Transform transform)
        {
            m_Transform = transform;
            m_Rigidbody = transform.gameObject.GetComponent<Rigidbody>();
        }

        public IEnumerator<IWorldTransform> GetEnumerator()
        {
            return ChildTransforms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => m_Transform.childCount;

        public string TransformName => m_Transform.name;

        public IWorldTransform? ParentTransform => m_Transform.parent ? new UnityTransform(m_Transform.parent) : null;

        public IReadOnlyCollection<IWorldTransform> ChildTransforms
        {
            get
            {
                var list = new List<IWorldTransform>();
                for (var i = 0; i < m_Transform.childCount; i++)
                {
                    var transform = m_Transform.GetChild(i);
                    list.Add(new UnityTransform(transform));
                }

                return list;
            }
        }

        public Vector3 Position => (m_Rigidbody?.position ?? m_Transform.position).ToSystemVector();

        public Vector3 Velocity => m_Rigidbody?.velocity.ToSystemVector() ?? Vector3.Zero;

        public Task<bool> SetVelocityAsync(Vector3 velocity)
        {
            async UniTask<bool> SetVelocityTask()
            {
                await UniTask.SwitchToMainThread();

                if (m_Rigidbody == null)
                {
                    return false;
                }

                m_Rigidbody.velocity = velocity.ToUnityVector();
                return true;
            }

            return SetVelocityTask().AsTask();
        }

        public virtual Task<bool> SetPositionAsync(Vector3 targetPosition)
        {
            async UniTask<bool> PositionTask()
            {
                if (!ValidationHelper.IsValid(targetPosition))
                {
                    return false;
                }
                
                await UniTask.SwitchToMainThread();

                var unityPosition = targetPosition.ToUnityVector();
                if (m_Rigidbody != null)
                {
                    m_Rigidbody.position = unityPosition;
                }
                else
                {
                    m_Transform.position = unityPosition;
                }

                return true;
            }

            return PositionTask().AsTask();
        }

        public Quaternion Rotation => (m_Rigidbody?.rotation ?? m_Transform.rotation).ToSystemQuaternion();

        public virtual Task<bool> SetRotationAsync(Quaternion rotation)
        {
            async UniTask<bool> RotationTask()
            {
                if (!ValidationHelper.IsValid(rotation))
                {
                    return false;
                }

                await UniTask.SwitchToMainThread();

                var unityRotation = rotation.ToUnityQuaternion();
                if (m_Rigidbody != null)
                {
                    m_Rigidbody.rotation = unityRotation;
                }
                else
                {
                    m_Transform.rotation = unityRotation;
                }

                return true;
            }

            return RotationTask().AsTask();
        }

        /// <inheritdoc />
        public Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();
                Object.Destroy(m_Transform.gameObject);
            }

            return DestroyTask().AsTask();
        }
    }
}