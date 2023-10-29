using Cysharp.Threading.Tasks;
using MoreLinq;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Unturned.Animals;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Zombies;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Entities
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedEntityDirectory : IEntityDirectory
    {
        public async Task<IReadOnlyCollection<IEntity>> GetEntitiesAsync()
        {
            await UniTask.SwitchToMainThread();

            var entities = new List<IEntity>();

            entities.AddRange(Provider.clients
                .Where(x => x?.player != null)
                .Select(x => new UnturnedPlayer(x.player)));

            entities.AddRange(AnimalManager.animals
                .Where(x => x != null)
                .Select(x => new UnturnedAnimal(x)));

            entities.AddRange(ZombieManager.tickingZombies
                .Where(x => x != null)
                .Select(x => new UnturnedZombie(x)));

            return entities;
        }

        public Task<IReadOnlyCollection<IEntityAsset>> GetEntityAssetsAsync()
        {
            var list = new List<AnimalAsset>();
            Assets.find(list);

            var assets = list.ConvertAll<IEntityAsset>(a => new UnturnedAnimalAsset(a));
            assets.Add(UnturnedPlayerAsset.Instance);
            assets.Add(UnturnedZombieAsset.Instance);

            return Task.FromResult<IReadOnlyCollection<IEntityAsset>>(assets);
        }
    }
}