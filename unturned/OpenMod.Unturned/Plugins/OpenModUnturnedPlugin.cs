using OpenMod.UnityEngine.Plugins;
using System;

namespace OpenMod.Unturned.Plugins
{
    /// <summary>
    /// Base class for all Unturned plugins.
    /// </summary>
    public abstract class OpenModUnturnedPlugin : OpenModUnityEnginePlugin
    {
        protected OpenModUnturnedPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}