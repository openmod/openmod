using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Rust.Players
{
    public class RustPlayerAsset : IEntityAsset
    {
        public BaseEntity Entity { get; }

        public RustPlayerAsset(BasePlayer entity)
        {
            Entity = entity;
            Name = entity.PrefabName;
            EntityAssetId = entity.prefabID.ToString();
            EntityType = entity.Family.ToString();
        }

        public string Name { get; }

        public string EntityAssetId { get; }

        public string EntityType { get; }
    }
}