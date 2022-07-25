using OpenMod.API.Commands;
using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Items
{
    public class UnturnedItemAssetCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        private readonly IItemDirectory m_ItemDirectory;

        public UnturnedItemAssetCommandParameterResolveProvider(IItemDirectory itemDirectory)
        {
            m_ItemDirectory = itemDirectory;
        }

        public bool Supports(Type type)
        {
            return type == typeof(UnturnedItemAsset) || type == typeof(IItemAsset) || type == typeof(ItemAsset);
        }

        public async Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var itemAsset = await m_ItemDirectory.FindByIdAsync(input) ??
                            await m_ItemDirectory.FindByNameAsync(input, false);

            if (type == typeof(IItemAsset))
            {
                return itemAsset;
            }

            var unturnedItemAsset = itemAsset as UnturnedItemAsset;

            if (type == typeof(UnturnedItemAsset))
            {
                return unturnedItemAsset;
            }

            if (type == typeof(ItemAsset))
            {
                return unturnedItemAsset?.ItemAsset;
            }

            throw new InvalidOperationException($"Unable to return type {type}");
        }
    }
}
