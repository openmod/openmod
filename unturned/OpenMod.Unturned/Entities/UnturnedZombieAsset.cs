using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Unturned.Entities
{
    public class UnturnedZombieAsset : IEntityAsset
    {
        private static UnturnedZombieAsset m_Instance;
        public static UnturnedZombieAsset Instance
        {
            get
            {
                return m_Instance ??= new UnturnedZombieAsset();
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