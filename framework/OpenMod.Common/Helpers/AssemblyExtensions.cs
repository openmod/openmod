using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenMod.Common.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Safely returns the set of loadable types from an assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to load types.</param>
        /// <returns>
        /// The set of types from the <paramref name="assembly" />, or the subset
        /// of types that could be loaded if there was any error.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="assembly" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Avoid using this method, unless you don't care about missing types
        /// </remarks>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }

        private static readonly Regex s_MissingFileAssemblyVersionRegex =
            new("'(?<assembly>\\S+?), Version=(?<version>.+?), ",
                RegexOptions.Compiled);

        private static readonly Regex s_TypeLoadAssemblyVersionRegex =
            new("assembly:(?<assembly>\\S+?), Version=(?<version>.+?), ",
                RegexOptions.Compiled);

        /// <summary>
        /// Gets the missing assembly names based on ReflectionTypeLoadException
        /// </summary>
        public static IEnumerable<AssemblyName> GetMissingDependencies(this ReflectionTypeLoadException reflectionTypeLoadException)
        {
            if (reflectionTypeLoadException == null)
            {
                throw new ArgumentNullException(nameof(reflectionTypeLoadException));
            }

            var missingAssemblies = new Dictionary<string, Version>();
            var loaderExceptions = reflectionTypeLoadException.LoaderExceptions;
            foreach (var loaderException in loaderExceptions)
            {
                //TypeLoadException is just matching with MissingFileAssemblyVersionRegex
                var match = s_MissingFileAssemblyVersionRegex.Match(loaderException.Message);
                if (!match.Success)
                    match = s_TypeLoadAssemblyVersionRegex.Match(loaderException.Message);

                if (!match.Success)
                    continue;
                
                var assemblyName = match.Groups["assembly"].Value;
                var version = Version.Parse(match.Groups["version"].Value);

                if (missingAssemblies.TryGetValue(assemblyName, out var currentVersion) && currentVersion >= version)
                    continue;

                missingAssemblies[assemblyName] = version;
            }

            return missingAssemblies.Select(s => new AssemblyName(s.Key) { Version = s.Value });
        }

        public static string GetNameVersion(this Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            return $"{assemblyName.Name} Version={assemblyName.Version}";
        }

        public static string GetNameVersion(this AssemblyName assemblyName)
        {
            return $"{assemblyName.Name} Version={assemblyName.Version}";
        }
    }
}