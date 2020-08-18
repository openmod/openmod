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
        public string BuildableAssetId { get; }

        public string BuildableType { get; }

        public UnturnedBuildableAsset(Asset asset)
        {
            BuildableAssetId = asset.id.ToString();
            BuildableType = asset.GetType().Name;
        }
    }
}