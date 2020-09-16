using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using HarmonyLib;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Shared
{
    public sealed class OpenModSharedUnturnedModule
    {
        private const string c_HarmonyInstanceId = "com.get-openmod.unturned.module";
        private readonly Dictionary<string, Assembly> m_LoadedAssemblies = new Dictionary<string, Assembly>();
        private Harmony m_HarmonyInstance;
        private readonly string[] m_IncompatibleModules = { "Rocket.Unturned", "Redox.Unturned" };
        private readonly string[] m_CompatibleModules = { "AviRockets" };

        private readonly Dictionary<string, Assembly> m_ResolvedAssemblies = new Dictionary<string, Assembly>();

        public bool Initialize(Assembly moduleAssembly)
        {
            var moduleAssemblyLocation = moduleAssembly.Location;
            var openModDirPath = Path.GetDirectoryName(moduleAssemblyLocation);
            var modulesDirectory = Directory.GetParent(openModDirPath).FullName;
            if (HasIncompatibleModules(Path.GetFileName(openModDirPath), modulesDirectory))
            {
                return false;
            }

            m_HarmonyInstance = new Harmony(c_HarmonyInstanceId);
            m_HarmonyInstance.PatchAll(GetType().Assembly);

            InstallNewtonsoftJson(openModDirPath);
            InstallTlsWorkaround();
            InstallAssemblyResolver();

            // ReSharper disable once AssignNullToNotNullAttribute
            foreach (var file in Directory.GetFiles(openModDirPath))
            {
                if (file.EndsWith(".dll"))
                {
                    LoadAssembly(openModDirPath, file);
                }
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

                    if (moduleDirName.Equals("Rocket.Unturned", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("RocketMod detected! Read how to migrate from RocketMod:");
                        Console.WriteLine("https://openmod.github.io/openmod-docs/user-guide/migration/rocketmod/");
                    }
                    else
                    {
                        Console.WriteLine("Please remove the module in order to use OpenMod.");
                    }

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
            Console.WriteLine("Location: " + typeof(IModuleNexus).Assembly.Location);
            var managedDir = Path.GetDirectoryName(typeof(IModuleNexus).Assembly.Location);

            // ReSharper disable once AssignNullToNotNullAttribute
            var unturnedNewtonsoftFile = Path.GetFullPath(Path.Combine(managedDir, "Newtonsoft.Json.dll"));
            var newtonsoftBackupFile = unturnedNewtonsoftFile + ".bak";
            var openModNewtonsoftFile = Path.GetFullPath(Path.Combine(openModDir, "Newtonsoft.Json.dll"));
            // ReSharper restore once AssignNullToNotNullAttribute

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
            var asm = AssemblyName.GetAssemblyName(unturnedNewtonsoftFile);
            GetVersionIndependentName(asm.FullName, out var version);
            if (!version.StartsWith("7.", StringComparison.OrdinalIgnoreCase))
                return;

            if (File.Exists(newtonsoftBackupFile))
            {
                File.Delete(newtonsoftBackupFile);
            }

            File.Move(unturnedNewtonsoftFile, newtonsoftBackupFile);
            File.Copy(openModNewtonsoftFile, unturnedNewtonsoftFile);
        }

        private void InstallAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                var name = GetVersionIndependentName(args.Name, out _);
                if (m_ResolvedAssemblies.ContainsKey(name))
                {
                    return m_ResolvedAssemblies[name];
                }

                var assemblies = m_LoadedAssemblies.Values
                    .Where(d => GetVersionIndependentName(d.FullName, out _).Equals(name))
                    .OrderByDescending(d => d.GetName().Version);
                
                var match= assemblies.FirstOrDefault();
                if (match != null)
                {
                    m_ResolvedAssemblies.Add(name, match);
                }

                return match;
            };
        }

        public void Shutdown()
        {
            m_HarmonyInstance?.UnpatchAll(c_HarmonyInstanceId);
        }

        private void InstallTlsWorkaround()
        {
            //http://answers.unity.com/answers/1089592/view.html
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationWorkaroundCallback;
        }

        private bool CertificateValidationWorkaroundCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                {
                    if (chainStatus.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        private static readonly Regex s_VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);

        private static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = s_VersionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return s_VersionRegex.Replace(fullAssemblyName, string.Empty);
        }

        public void LoadAssembly(string baseDirectory, string dllName)
        {
            //Load the dll from the same directory as this assembly
            var dllFullPath = Path.GetFullPath(Path.Combine(baseDirectory, dllName));

            if (string.Equals(baseDirectory, dllFullPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var data = File.ReadAllBytes(dllFullPath);
            var asm = Assembly.Load(data);

            var name = GetVersionIndependentName(asm.FullName, out _);

            if (m_LoadedAssemblies.ContainsKey(name))
            {
                return;
            }

            m_LoadedAssemblies.Add(name, asm);
        }

        public void OnPostInitialize()
        {
 
        }
    }
}