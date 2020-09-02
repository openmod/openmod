using Cysharp.Threading.Tasks;
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
        public Task<IReadOnlyCollection<IEntity>> GetEntitiesAsync()
        {
            // todo: add all zombies, players and animals
            throw new System.NotImplementedException();
        }

        public async Task<IReadOnlyCollection<IEntityAsset>> GetEntityAssetsAsync()
        {
            await UniTask.SwitchToMainThread();

            var assets = new List<IEntityAsset>();

            assets.AddRange(Assets.find(EAssetType.ANIMAL)
                .Cast<AnimalAsset>()
                .Select(d => new UnturnedAnimalAsset(d)));

            assets.Add(UnturnedPlayerAsset.Instance);
            assets.Add(UnturnedZombieAsset.Instance);

            return assets;
        }
    }
}