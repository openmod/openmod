using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Rust.Transforms;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Rust.Entities
{
    public class RustEntity : IEntity, IDamageSource
    {
        public BaseEntity Entity { get; }

        public RustEntity(BaseEntity entity)
        {
            Entity = entity;
            Transform = new RustNetworkableTransform(entity);
            EntityInstanceId = entity.GetInstanceID().ToString();
            State = new RustEntityState(entity);
        }

        public IWorldTransform Transform { get; }

        public virtual IEntityAsset Asset
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEntityState State { get; }

        public string EntityInstanceId { get; protected set; }

        public virtual Task<bool> SetPositionAsync(Vector3 position)
        {
            return SetPositionAsync(position, Transform.Rotation);
        }

        public virtual Task<bool> SetPositionAsync(Vector3 position, Quaternion rotation)
        {
            async UniTask<bool> TeleportTask()
            {
                await UniTask.SwitchToMainThread();

                var entity = Entity;

                var combatEntity = entity as BaseCombatEntity;
                if (combatEntity != null && !combatEntity.IsAlive())
                {
                    return false;
                }

                return DoTeleport(position, rotation);
            }

            return TeleportTask().AsTask();
        }

        protected virtual bool DoTeleport(Vector3 destination, Quaternion rotation)
        {
            throw new NotImplementedException();
        }

        public virtual string DamageSourceName
        {
            get
            {
                return Entity.ShortPrefabName;
            }
        }
    }
}
