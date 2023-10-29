using System;
using System.Reflection;
using SDG.Framework.Modules;

namespace OpenMod.Unturned.Module.Shared
{
    public static class SystemDrawingRedirect
    {
        public static void Install()
        {
            ModuleHook.PreVanillaAssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly? OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("System.Drawing"))
            {
                return typeof(SystemDrawingRedirect).Assembly;
            }

            return null;
        }

        public static void Uninstall()
        {
            ModuleHook.PreVanillaAssemblyResolve -= OnAssemblyResolve;
        }
    }
}
