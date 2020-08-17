using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Unturned.Entities
{
    public class UnturnedPlayerAsset : IEntityAsset
    {
        private static UnturnedPlayerAsset m_Instance;
        public static UnturnedPlayerAsset Instance
        {
            get
            {
                return m_Instance ??= new UnturnedPlayerAsset();
            }
        }

        private UnturnedPlayerAsset()
        {
            Name = "Player";
            EntityAssetId = "1";
            EntityType = UnturnedEntityTypes.Player;
        }

        public string Name { get; }
        
        public string EntityAssetId { get; }

        public string EntityType { get; }
    }
}