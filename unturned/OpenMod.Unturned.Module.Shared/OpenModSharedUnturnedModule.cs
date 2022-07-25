using HarmonyLib;
using OpenMod.Common.Helpers;
using OpenMod.NuGet;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace OpenMod.Unturned.Module.Shared
{
    public sealed class OpenModSharedUnturnedModule
    {
        private const string c_HarmonyInstanceId = "com.get-openmod.unturned.module";
        private readonly Dictionary<string, Assembly> m_LoadedAssemblies = new();
        private readonly Dictionary<string, Assembly> m_ResolvedAssemblies = new();
        private readonly string[] m_IncompatibleModules = { "Redox.Unturned" };
        private readonly string[] m_CompatibleModules = { "AviRockets", "uScript.Unturned", "Rocket.Unturned" };
        private RemoteCertificateValidationCallback? m_OldCallBack;
        private Harmony? m_HarmonyInstance;
        private Assembly? m_ModuleAssembly;
        public bool Initialize(Assembly moduleAssembly, bool isDynamicLoad)
        {
            m_ModuleAssembly = moduleAssembly;

            var modulesDirectory = Path.Combine(ReadWrite.PATH, "Modules");
            var openModDirPath = Path.GetDirectoryName(Directory
                .GetFiles(modulesDirectory, "OpenMod.Unturned.Module.Shared.dll", SearchOption.AllDirectories)
                .FirstOrDefault() ?? throw new Exception("Failed to find OpenMod directory"))!;

            if (HasIncompatibleModules(Path.GetFileName(openModDirPath), modulesDirectory))
            {
                return false;
            }

            m_HarmonyInstance = new Harmony(c_HarmonyInstanceId);
            m_HarmonyInstance.PatchAll(GetType().Assembly);

            InstallNewtonsoftJson(openModDirPath);

            var rocketUnturnedFile = Directory
                .GetFiles(modulesDirectory, "Rocket.Unturned.module", SearchOption.AllDirectories)
                .FirstOrDefault();

            var rocketModDirectory = rocketUnturnedFile != null
                ? Path.GetDirectoryName(rocketUnturnedFile)!
                : null;

            if (rocketModDirectory != null)
            {
                foreach (var file in Directory.GetFiles(rocketModDirectory, "*.dll"))
                {
                    var assembly = Assembly.LoadFile(file);
                    m_ResolvedAssemblies.Add(ReflectionExtensions.GetVersionIndependentName(assembly.FullName), assembly);
                }
            }

            if (!isDynamicLoad)
            {
                SystemDrawingRedirect.Install();
                InstallTlsWorkaround();
                InstallAssemblyResolver();
            }

            foreach (var file in Directory.GetFiles(openModDirPath, "*.dll"))
            {
                LoadAssembly(openModDirPath, file);
            }

            return true;
        }

        private bool HasIncompatibleModules(string openModDirName, string modulesDirectory)
        {
            foreach (var modulePath in Directory.GetDirectories(modulesDirectory))
            {
                var moduleDirName = Path.GetFileName(modulePath);
                // ReSharper disable once PossibleNullReferenceException
                if (moduleDirName.Equals(openModDirName))
                {
                    continue; // OpenMod's own directory
                }

                if (m_CompatibleModules.Any(d => d.Equals(moduleDirName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var previousColor = Console.ForegroundColor;
                if (m_IncompatibleModules.Any(d => d.Equals(moduleDirName, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("================================================================");
                    Console.WriteLine($"Incompatible module detected: {moduleDirName}");
                    Console.WriteLine("Please remove the module in order to use OpenMod.");
                    Console.WriteLine("OpenMod will abort loading.");
                    Console.WriteLine("================================================================");
                    Console.ForegroundColor = previousColor;
                    return true;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Unknown module detected: {moduleDirName}");
                Console.WriteLine("This module may conflict with OpenMod.");
                Console.WriteLine("OpenMod may not work correctly.");
                Console.ForegroundColor = previousColor;
            }

            return false;
        }

        private void InstallNewtonsoftJson(string openModDir)
        {
            var managedDir = Path.GetDirectoryName(typeof(IModuleNexus).Assembly.Location)!;

            var unturnedNewtonsoftFile = Path.GetFullPath(Path.Combine(managedDir, "Newtonsoft.Json.dll"));
            var newtonsoftBackupFile = unturnedNewtonsoftFile + ".bak";
            var openModNewtonsoftFile = Path.GetFullPath(Path.Combine(openModDir, "Newtonsoft.Json.dll"));

            const string runtimeSerialization = "System.Runtime.Serialization.dll";
            var unturnedRuntimeSerialization = Path.GetFullPath(Path.Combine(managedDir, runtimeSerialization));
            var openModRuntimeSerialization = Path.GetFullPath(Path.Combine(openModDir, runtimeSerialization));

            const string xmlLinq = "System.Xml.Linq.dll";
            var unturnedXmlLinq = Path.GetFullPath(Path.Combine(managedDir, xmlLinq));
            var openModXmlLinq = Path.GetFullPath(Path.Combine(openModDir, xmlLinq));

            // Copy Libraries of Newtonsoft.Json
            if (!File.Exists(unturnedRuntimeSerialization))
            {
                File.Copy(openModRuntimeSerialization, unturnedRuntimeSerialization);
            }

            if (!File.Exists(unturnedXmlLinq))
            {
                File.Copy(openModXmlLinq, unturnedXmlLinq);
            }

            // Copy Newtonsoft.Json
            var assembly = AssemblyName.GetAssemblyName(unturnedNewtonsoftFile);
            ReflectionExtensions.GetVersionIndependentName(assembly.FullName, out var version);
            if (!version.StartsWith("7.", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(newtonsoftBackupFile))
            {
                File.Delete(newtonsoftBackupFile);
            }

            File.Move(unturnedNewtonsoftFile, newtonsoftBackupFile);
            File.Copy(openModNewtonsoftFile, unturnedNewtonsoftFile);
        }

        private void InstallAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private Assembly? OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = ReflectionExtensions.GetVersionIndependentName(args.Name);
            if (m_ResolvedAssemblies.ContainsKey(name))
            {
                return m_ResolvedAssemblies[name];
            }

            var matcher = new Func<Assembly, bool>(d => ReflectionExtensions.GetVersionIndependentName(d.FullName).Equals(name));
            var match = m_LoadedAssemblies.Values
                .OrderByDescending(d => d.GetName().Version)
                .FirstOrDefault(matcher);

            if (match != null)
            {
                m_ResolvedAssemblies.Add(name, match);
            }

            return AppDomain.CurrentDomain.GetAssemblies()
                .OrderByDescending(d => d.GetName().Version)
                .FirstOrDefault(matcher);
        }

        public void Shutdown()
        {
            ServicePointManager.ServerCertificateValidationCallback = m_OldCallBack;
            SystemDrawingRedirect.Uninstall();
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            m_HarmonyInstance?.UnpatchAll(c_HarmonyInstanceId);
        }

        private void InstallTlsWorkaround()
        {
            m_OldCallBack = ServicePointManager.ServerCertificateValidationCallback;

            //http://answers.unity.com/answers/1089592/view.html
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationWorkaroundCallback;
        }

        private bool CertificateValidationWorkaroundCallback(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            foreach (var chainStatus in chain.ChainStatus)
            {
                if (chainStatus.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;

                    var chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }

            return isOk;
        }

        public void LoadAssembly(string baseDirectory, string dllName)
        {
            //Load the dll from the same directory as this assembly
            var dllFullPath = Path.GetFullPath(Path.Combine(baseDirectory, dllName));

            if (string.Equals(baseDirectory, dllFullPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (dllName.Equals("OpenMod.API.dll") && !m_ModuleAssembly!.FullName.Contains(".Dev"))
            {
                try
                {
                    File.Delete(dllFullPath);
                }
                catch
                {
                    // ignored 
                    return;
                }
            }

            var data = File.ReadAllBytes(dllFullPath);
            var asm = Assembly.Load(data);

            var name = ReflectionExtensions.GetVersionIndependentName(asm.FullName, out _);

            if (m_LoadedAssemblies.ContainsKey(name))
            {
                return;
            }

            m_LoadedAssemblies.Add(name, asm);
        }

        public void OnPostInitialize()
        {
        }

        public NuGetPackageManager GetNugetPackageManager(string openModDirectory)
        {
            try
            {
                var packagesDirectory = Path.Combine(openModDirectory, "packages");
                if (!Directory.Exists(packagesDirectory))
                {
                    Directory.CreateDirectory(packagesDirectory);
                }

                var nugetPackageManager = new NuGetPackageManager(packagesDirectory)
                { Logger = new NuGetConsoleLogger() };

                // these dependencies are not required and cause issues
                nugetPackageManager.IgnoreDependencies(
                    "Microsoft.NETCore.Platforms",
                    "Microsoft.Packaging.Tools",
                    "NETStandard.Library",
                    "OpenMod.UnityEngine.Redist",
                    "OpenMod.Unturned.Redist",
                    "System.IO.FileSystem.Watcher");

                return nugetPackageManager;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
