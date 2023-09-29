using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Zombies
{
    public class UnturnedZombieAsset : IEntityAsset
    {
        private static UnturnedZombieAsset? s_Instance;
        public static UnturnedZombieAsset Instance
        {
            get
            {
                return s_Instance ??= new UnturnedZombieAsset();
            }
        }

        private UnturnedZombieAsset()
        {
            Name = "Zombie";
            EntityAssetId = "1";
            EntityType = UnturnedEntityTypes.Zombie;
        }

        public string Name { get; }

        public string EntityAssetId { get; }

        public string EntityType { get; }
    }
}