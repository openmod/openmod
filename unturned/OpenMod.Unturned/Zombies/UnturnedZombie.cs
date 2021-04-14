using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Zombies
{
    public class UnturnedZombie : IEntity, IHasHealth, IDamageSource
    {
        private static readonly FieldInfo m_HealthField;
        private static readonly FieldInfo m_MaxHealthField;

        static UnturnedZombie()
        {
            m_HealthField = typeof(Zombie).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic);
            m_MaxHealthField = typeof(Zombie).GetField("maxHealth", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public UnturnedZombie(Zombie zombie)
        {
            Zombie = zombie;
            Asset = UnturnedZombieAsset.Instance;
            State = NullEntityState.Instance;
            Transform = new UnityTransform(zombie.transform);
            EntityInstanceId = zombie.id.ToString();
        }

        public Zombie Zombie { get; }

        public IEntityAsset Asset { get; }

        public IEntityState State { get; }

        public IWorldTransform Transform { get; }

        public string EntityInstanceId { get; }

        public bool IsAlive => !Zombie.isDead;

        public double MaxHealth => (ushort)m_MaxHealthField.GetValue(Zombie);

        public double Health => (ushort)m_HealthField.GetValue(Zombie);

        public Task SetHealthAsync(double health)
        {
            async UniTask SetHeathTask()
            {
                await UniTask.SwitchToMainThread();
                m_HealthField.SetValue(Zombie, (ushort)health);
            }

            return SetHeathTask().AsTask();
        }

        public Task DamageAsync(double amount)
        {
            async UniTask DamageTask()
            {
                await UniTask.SwitchToMainThread();

                DamageTool.damageZombie(new DamageZombieParameters
                {
                    zombie = Zombie,
                    damage = (float)amount,
                    times = 1,
                }, out _, out _);
            }

            return DamageTask().AsTask();
        }

        public Task KillAsync()
        {
            return DamageAsync(float.MaxValue);
        }

        public Task<bool> SetPositionAsync(Vector3 position)
        {
            return SetPositionAsync(position, Transform.Rotation);
        }

        public async Task<bool> SetPositionAsync(Vector3 position, Quaternion rotation)
        {
            // should be synced automatically

            if (Transform.Position != position && !await Transform.SetPositionAsync(position))
            {
                return false;
            }

            if (Transform.Rotation != rotation && !await Transform.SetRotationAsync(rotation))
            {
                return false;
            }

            return true;
        }

        public string DamageSourceName
        {
            get
            {
                return Asset.Name;
            }
        }
    }
}