using System;
using OpenMod.UnityEngine.Plugins;

namespace OpenMod.Unturned.Plugins
{
    public abstract class OpenModUnturnedPlugin : OpenModUnityEnginePlugin
    {
        protected OpenModUnturnedPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}