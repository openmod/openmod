using System;
using System.Text;

namespace OpenMod.Extensions.Games.Abstractions.Buildings
{
    public interface IBuildableAsset
    {
        string BuildableAssetId { get; }

        string BuildableType { get; }
    }
}
