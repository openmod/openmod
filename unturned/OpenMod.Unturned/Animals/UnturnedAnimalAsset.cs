using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Animals
{
    public class UnturnedAnimalAsset : IEntityAsset
    {
        public AnimalAsset AnimalAsset { get; }

        public UnturnedAnimalAsset(AnimalAsset animalAsset)
        {
            AnimalAsset = animalAsset;
            Name = animalAsset.animalName;
            EntityAssetId = animalAsset.id.ToString();
            EntityType = UnturnedEntityTypes.Animal;
        }

        public string Name { get; }

        public string EntityAssetId { get; }

        public string EntityType { get; }
    }
}