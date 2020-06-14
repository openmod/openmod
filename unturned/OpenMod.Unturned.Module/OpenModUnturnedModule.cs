using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using SDG.Framework.Modules;
using HarmonyLib;

namespace OpenMod.Unturned.Module
{
    public class OpenModUnturnedModule : IModuleNexus
    {
        private const string HarmonyInstanceId = "com.get-openmod.unturned.module";
        private readonly Dictionary<string, Assembly> m_LoadedAssemblies = new Dictionary<string, Assembly>();
        private Harmony m_HarmonyInstance;

        public void initialize()
        {
            var selfAssembly = typeof(OpenModUnturnedModule).Assembly;

            m_HarmonyInstance = new Harmony(HarmonyInstanceId);
            m_HarmonyInstance.PatchAll(selfAssembly);

            InstallNewtonsoftJson();
            InstallTlsWorkaround();
            InstallAssemblyResolver();

            var assemblyLocation = selfAssembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            foreach (var file in Directory.GetFiles(assemblyDirectory ?? throw new InvalidOperationException()))
            {
                if (file.EndsWith(".dll"))
                {
                    LoadAssembly(file);
                }
            }

            OpenModInitializer.Initialize();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void InstallNewtonsoftJson()
        {
            var managedDir = Path.GetDirectoryName(typeof(IModuleNexus).Assembly.Location);
            var openModDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var unturnedNewtonsoftFile = Path.GetFullPath(Path.Combine(managedDir ?? throw new InvalidOperationException(), "Newtonsoft.Json.dll"));
            var newtonsoftBackupFile = unturnedNewtonsoftFile + ".bak";
            var openModNewtonsoftFile = Path.GetFullPath(Path.Combine(openModDir ?? throw new InvalidOperationException(), "Newtonsoft.Json.dll"));

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

                if (m_LoadedAssemblies.ContainsKey(name))
                {
                    return m_LoadedAssemblies[name];
                }

                return null;
            };
        }

        public void shutdown()
        {
            m_HarmonyInstance.UnpatchAll(HarmonyInstanceId);
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


        private static readonly Regex VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);

        protected static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = VersionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return VersionRegex.Replace(fullAssemblyName, string.Empty);
        }

        public void LoadAssembly(string dllName)
        {
            //Load the dll from the same directory as this assembly
            var selfLocation = typeof(OpenModUnturnedModule).Assembly.Location;
            var currentPath = Path.GetDirectoryName(selfLocation) ?? string.Empty;
            var dllFullPath = Path.GetFullPath(Path.Combine(currentPath, dllName));

            if (string.Equals(selfLocation, dllFullPath, StringComparison.OrdinalIgnoreCase))
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
    }
}