using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OpenMod.API;
using OpenMod.UnityEngine.Plugins;
using Oxide.Core;
using Oxide.Core.Extensions;
using OxideExtension = Oxide.Core.Extensions.Extension;

namespace OpenMod.Rust.Oxide.Extension.Dev
{
    public class OpenModRustOxideDevExtension : OxideExtension
    {
        public OpenModRustOxideDevExtension(ExtensionManager manager) : base(manager)
        {
            var assemblyVersion = GetType().Assembly.GetName().Version;
            Version = new VersionNumber(assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
        }

        public override void Load()
        {
            var openModDirectory = Path.Combine(Interface.Oxide.RootDirectory, "openmod");
            if (!Directory.Exists(openModDirectory))
            {
                Directory.CreateDirectory(openModDirectory);
            }

            var parameters = new RuntimeInitParameters
            {
                CommandlineArgs = Environment.GetCommandLineArgs(),
                WorkingDirectory = openModDirectory
            };

            var assemblies = new List<Assembly>
            {
                typeof(OpenModUnityEnginePlugin).Assembly,
                typeof(OpenModRustOxideHost).Assembly,
                typeof(BaseOpenModRustHost).Assembly
            };

            var openModRuntime = new Runtime.Runtime();
            OpenModRuntime = openModRuntime;
            openModRuntime.Init(assemblies, parameters);
        }

        public IRuntime OpenModRuntime { get; private set; }

        public override string Name { get; } = "OpenMod for Rust OxideMod";

        public override string Author { get; } = "OpenMod Contributors";

        public override VersionNumber Version { get; }
    }
}

