using AsmResolver.IO;
using AsmResolver.PE;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.DotNet.Builder;
using AsmResolver.PE.DotNet.Metadata.Strings;
using AsmResolver.PE.DotNet.Metadata.Tables;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using OpenMod.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenMod.Common.Hotloading
{
    /// <summary>
    /// Adds support for hotloading assemblies.
    /// Use <see cref="LoadAssembly(byte[])"/> instead of <see cref="Assembly.Load(byte[])"/>.
    /// </summary>
    public static class Hotloader
    {
        private static readonly Dictionary<AssemblyName, Assembly> s_Assemblies;
        private static readonly bool s_IsMono;

        /// <summary>
        /// Defines if hotloading is enabled.
        /// </summary>
        public static bool Enabled { get; set; }

        static Hotloader()
        {
            s_Assemblies = new Dictionary<AssemblyName, Assembly>(AssemblyNameEqualityComparer.Instance);
            s_IsMono = Type.GetType("Mono.Runtime") is not null;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly? OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return FindAssembly(new(args.Name));
        }

        /// <summary>
        /// Hotloads an assembly. Redirects to <see cref="Assembly.Load(byte[])"/> if <see cref="Enabled"/> is set to false.
        /// </summary>
        /// <param name="assemblyData">The assembly to hotload.</param>
        /// <returns>The loaded assembly.</returns>
        public static Assembly LoadAssembly(byte[] assemblyData)
        {
            return LoadAssembly(assemblyData, assemblySymbols: null);
        }

        /// <summary>
        /// Hotloads an assembly. Redirects to <see cref="Assembly.Load(byte[], byte[])"/> if <see cref="Enabled"/> is set to false.
        /// </summary>
        /// <param name="assemblyData">The assembly to hotload.</param>
        /// <param name="assemblySymbols">A byte array that contains the raw bytes representing the symbols for the assembly.</param>
        /// <returns>The loaded assembly.</returns>
        public static Assembly LoadAssembly(byte[] assemblyData, byte[]? assemblySymbols)
        {
            if (!Enabled)
            {
                return Assembly.Load(assemblyData, assemblySymbols);
            }

            var image = PEImage.FromBytes(assemblyData);
            var isStrongNamed = (image.DotNetDirectory!.Flags & DotNetDirectoryFlags.StrongNameSigned) == DotNetDirectoryFlags.StrongNameSigned;

            if (!s_IsMono && isStrongNamed)
            {
                // Don't hotload strong-named assemblies unless mono
                // Will cause FileLoadException's if not mono
                return Assembly.Load(assemblyData, assemblySymbols);
            }

            var metadata = image.DotNetDirectory.Metadata!;
            var tablesStream = metadata.GetStream<TablesStream>();
            var oldStringsStream = metadata.GetStream<StringsStream>();

            // get reference to assembly def row
            ref var assemblyRow = ref tablesStream
                .GetTable<AssemblyDefinitionRow>(TableIndex.Assembly)
                .GetRowRef(1);

            // get original name
            string name = oldStringsStream.GetStringByIndex(assemblyRow.Name)!;

            // structure full name
            var version = new Version(assemblyRow.MajorVersion, assemblyRow.MinorVersion, assemblyRow.BuildNumber, assemblyRow.RevisionNumber);
            var realFullName = $"{name}, Version={version}, Culture=neutral, PublicKeyToken=null";
            var realAssemblyName = new AssemblyName(realFullName);

            // generate new name
            var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
            var newName = $"{name}-{guid}";

            // update assembly def name
            assemblyRow.Name = oldStringsStream.GetPhysicalSize();
            using var output = new MemoryStream();

            var writer = new BinaryStreamWriter(output);

            writer.WriteBytes(oldStringsStream.CreateReader().ReadToEnd());
            writer.WriteBytes(Encoding.UTF8.GetBytes(newName));
            writer.WriteByte(0); // Add Null Terminator
            writer.Align(4);

            var newStringsStream = new SerializedStringsStream(output.ToArray());
            // strings index size may have changed, updating in tables stream
            tablesStream.StringIndexSize = newStringsStream.IndexSize;

            // replace old strings with new one
            metadata.Streams[metadata.Streams.IndexOf(oldStringsStream)] = newStringsStream;

            var builder = new ManagedPEFileBuilder();
            // reuse old output stream
            output.SetLength(0);

            builder.CreateFile(image).Write(output);

            var newAssemblyData = output.ToArray();

            var assembly = Assembly.Load(newAssemblyData, assemblySymbols);
            s_Assemblies[realAssemblyName] = assembly;
            return assembly;
        }

        /// <summary>
        /// Removes an assembly from the hotloader cache.
        /// </summary>
        /// <param name="assembly">The assembly to remove.</param>
        public static void Remove(Assembly assembly)
        {
            foreach (var key in s_Assemblies
                .Where(kv => kv.Value == assembly)
                .Select(x => x.Key)
                .ToArray())
            {
                s_Assemblies.Remove(key);
            }
        }

        /// <summary>
        /// Resolves a hotloaded assembly. Hotloaded assemblies have an auto generated assembly name.
        /// </summary>
        /// <param name="fullname">The assembly name to resolve.</param>
        /// <returns><b>The hotloaded assembly</b> if found; otherwise, <b>null</b>.</returns>
        [Obsolete("Use " + nameof(FindAssembly) + " method instead")]
        public static Assembly? GetAssembly(string fullname)
        {
            return FindAssembly(new AssemblyName(fullname));
        }

        /// <summary>
        /// Resolves a hotloaded assembly. Hotloaded assemblies have an auto generated assembly name.
        /// </summary>
        /// <param name="name">The assembly name to resolve.</param>
        /// <returns><b>The hotloaded assembly</b> if found; otherwise, <b>null</b>.</returns>
        public static Assembly? FindAssembly(AssemblyName name)
        {
            return s_Assemblies.TryGetValue(name, out var assembly) ? assembly : null;
        }

        /// <summary>
        /// Checks if hotloader contains assembly.
        /// </summary>
        /// <param name="name">The assembly name to check.</param>
        public static bool ContainsAssembly(AssemblyName name)
        {
            return s_Assemblies.ContainsKey(name);
        }

        /// <summary>
        /// Gets all hotloaded assemblies.
        /// </summary>
        /// <returns>The hotloaded assemblies.</returns>
        // ReSharper disable once UnusedMember.Global
        public static IReadOnlyCollection<Assembly> GetHotloadedAssemblies()
        {
            return s_Assemblies.Values;
        }

        /// <summary>
        /// Gets the real assembly name of an hotloaded assembly. Hotloaded assemblies have an auto generated assembly name.
        /// </summary>
        /// <param name="assembly">The assembly to get the real name of.</param>
        /// <returns><b>The real assembly name</b> of the hotloaded assembly. If the given assembly was not hotloaded, it will return <b>the assembly's name</b>.</returns>
        public static AssemblyName GetRealAssemblyName(Assembly assembly)
        {
            var assemblyName = s_Assemblies.FirstOrDefault(kv => kv.Value == assembly).Key;
            return assemblyName ?? assembly.GetName();
        }
    }
}
