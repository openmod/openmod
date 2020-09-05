using OpenMod.UnityEngine.Plugins;
using System;

namespace OpenMod.Unturned.Plugins
{
    public abstract class OpenModUnturnedPlugin : OpenModUnityEnginePlugin
    {
        protected OpenModUnturnedPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}