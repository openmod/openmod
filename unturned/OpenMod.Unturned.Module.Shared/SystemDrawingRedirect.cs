using System;
using System.Reflection;

namespace OpenMod.Unturned.Module.Shared
{
    public static class SystemDrawingRedirect
    {
        public static void Install()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("System.Drawing"))
            {
                return typeof(SystemDrawingRedirect).Assembly;
            }

            return null;
        }

        public static void Uninstall()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }
    }
}
