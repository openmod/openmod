using System;
using System.Linq;
using SDG.Unturned;

namespace Rocket.Unturned.Items
{
    public class Attachment{
        public ushort AttachmentId = 0;
        public byte Durability = 100;
        public Attachment(ushort attachmentId, byte durability){
            AttachmentId = attachmentId;
            Durability = durability;
        }
    }

    public static class UnturnedItems
    {
        public static ItemAsset GetItemAssetByName(string name)
        {
            if (String.IsNullOrEmpty(name)) return null;
            return SDG.Unturned.Assets.find(EAssetType.ITEM).Cast<ItemAsset>().Where(i => i.itemName != null && i.itemName.ToLower().Contains(name.ToLower())).FirstOrDefault();
        }

        public static ItemAsset GetItemAssetById(ushort id)
        {
            Asset asset = SDG.Unturned.Assets.find(EAssetType.ITEM, id);
            if (asset == null) return null;
            return (ItemAsset)asset;
        }

        public static Item AssembleItem(ushort itemId, byte clipsize, Attachment sight, Attachment tactical, Attachment grip, Attachment barrel, Attachment magazine, EFiremode firemode = EFiremode.SAFETY, byte amount = 1, byte durability = 100)
        {
            byte[] metadata = new byte[18];

            if (sight != null && sight.AttachmentId != 0)
            {
                byte[] sightBytes = BitConverter.GetBytes(sight.AttachmentId);
                metadata[0] = sightBytes[0];
                metadata[1] = sightBytes[1];
                metadata[13] = sight.Durability;
            }

            if (tactical != null && tactical.AttachmentId != 0)
            {
                byte[] tacticalBytes = BitConverter.GetBytes(tactical.AttachmentId);
                metadata[2] = tacticalBytes[0];
                metadata[3] = tacticalBytes[1];
                metadata[14] = tactical.Durability;
            }

            if (grip != null && grip.AttachmentId != 0)
            {
                byte[] gripBytes = BitConverter.GetBytes(grip.AttachmentId);
                metadata[4] = gripBytes[0];
                metadata[5] = gripBytes[1];
                metadata[15] = grip.Durability;
            }

            if (barrel != null && barrel.AttachmentId != 0)
            {
                byte[] barrelBytes = BitConverter.GetBytes(barrel.AttachmentId);
                metadata[6] = barrelBytes[0];
                metadata[7] = barrelBytes[1];
                metadata[16] = barrel.Durability;
            }

            if (magazine != null && magazine.AttachmentId != 0)
            {
                byte[] magazineBytes = BitConverter.GetBytes(magazine.AttachmentId);
                metadata[8] = magazineBytes[0];
                metadata[9] = magazineBytes[1];
                metadata[17] = magazine.Durability;
            }

            metadata[10] = clipsize;
            metadata[11] = (byte)firemode;
            metadata[12] = (byte)1;

            return AssembleItem(itemId,amount,durability,metadata);
        }

        public static Item AssembleItem(ushort itemId, byte amount = 1, byte durability = 100, byte[] metadata = null)
        {
            return new Item(itemId, amount, durability, (metadata == null ? new byte[0] : metadata));
        }
    }
}
