using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Animals
{
    public class UnturnedAnimal : IEntity, IHasHealth, IDamageSource
    {
        private static readonly FieldInfo m_HealthField;

        static UnturnedAnimal()
        {
            m_HealthField = typeof(Animal).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public UnturnedAnimal(Animal animal)
        {
            Animal = animal;
            Asset = new UnturnedAnimalAsset(animal.asset);
            State = NullEntityState.Instance;
            Transform = new UnityTransform(animal.transform);
            EntityInstanceId = animal.id.ToString();
        }
        public Animal Animal { get; }

        public IEntityAsset Asset { get; }

        public IEntityState State { get; }

        public IWorldTransform Transform { get; }

        public string EntityInstanceId { get; }

        public bool IsAlive => !Animal.isDead;

        public double MaxHealth => Animal.asset.health;

        public double Health => (ushort)m_HealthField.GetValue(Animal);

        public Task SetHealthAsync(double health)
        {
            async UniTask SetHeathTask()
            {
                await UniTask.SwitchToMainThread();
                m_HealthField.SetValue(Animal, (ushort)health);
            }

            return SetHeathTask().AsTask();
        }

        public Task DamageAsync(double amount)
        {
            async UniTask DamageTask()
            {
                await UniTask.SwitchToMainThread();

                DamageTool.damageAnimal(new DamageAnimalParameters
                {
                    animal = Animal,
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