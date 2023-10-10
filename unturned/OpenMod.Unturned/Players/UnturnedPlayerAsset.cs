using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Players
{
    public class UnturnedPlayerAsset : IEntityAsset
    {
        private static UnturnedPlayerAsset? s_Instance;
        public static UnturnedPlayerAsset Instance
        {
            get
            {
                return s_Instance ??= new UnturnedPlayerAsset();
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