using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Buildings;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;

namespace OpenMod.Unturned.Buildings
{
    public class UnturnedBuildableAsset : IBuildableAsset
    {
        public Asset Asset { get; }

        public string BuildableAssetId => Asset.id.ToString();

        public string BuildableType => Asset.GetType().Name;

        public UnturnedBuildableAsset(Asset asset)
        {
            Asset = asset;
        }
    }
}