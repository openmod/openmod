using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.Common.Hotloading;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Permissions;
using OpenMod.Core.Persistence;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.NuGet;
using Semver;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.Runtime
{
    [UsedImplicitly]
    [OpenModInternal]
    public sealed class Runtime : IRuntime
    {
        public Runtime()
        {
            Version = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
            HostAssemblies = new List<Assembly>();
        }

        public SemVersion Version { get; }

        public string OpenModComponentId { get; } = "OpenMod.Core";

        public bool IsComponentAlive => Status is not RuntimeStatus.Unloaded and not RuntimeStatus.Crashed;

        public ILifetimeScope LifetimeScope { get; private set; } = null!;

        public RuntimeStatus Status { get; private set; } = RuntimeStatus.Unloaded;

        public string WorkingDirectory { get; private set; } = null!;

        public string[] CommandlineArgs { get; private set; } = Array.Empty<string>();

        public IDataStore DataStore { get; private set; } = null!;

        public IHostInformation? HostInformation { get; private set; }

        public IReadOnlyCollection<Assembly> HostAssemblies { get; private set; }

        public IHost? Host { get; private set; }

        public bool IsDisposing { get; private set; }

        private DateTime? m_DateLogger;
        private ILoggerFactory? m_LoggerFactory;
        private ILogger<Runtime>? m_Logger;

        // these variables are used for soft reloads
        private List<Assembly>? m_OpenModHostAssemblies;
        private Func<IHostBuilder>? m_HostBuilderFunc;
        private RuntimeInitParameters? m_RuntimeInitParameters;

        public void Init(List<Assembly> openModAssemblies, RuntimeInitParameters parameters, Func<IHostBuilder>? hostBuilderFunc = null)
        {
            AsyncHelper.RunSync(() => InitAsync(openModAssemblies, parameters, hostBuilderFunc));
        }

        public async Task<IHost> InitAsync(
            List<Assembly> openModHostAssemblies,
            RuntimeInitParameters parameters,
            Func<IHostBuilder>? hostBuilderFunc = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            HostAssemblies = openModHostAssemblies ?? throw new ArgumentNullException(nameof(openModHostAssemblies));

            try
            {
                IsDisposing = false;

                var openModCoreAssembly = typeof(AsyncHelper).Assembly;
                if (!openModHostAssemblies.Contains(openModCoreAssembly))
                {
                    openModHostAssemblies.Insert(0, openModCoreAssembly);
                }

                m_OpenModHostAssemblies = openModHostAssemblies;
                m_HostBuilderFunc = hostBuilderFunc;
                m_RuntimeInitParameters = parameters;

                var hostBuilder = hostBuilderFunc == null ? new HostBuilder() : hostBuilderFunc();

                if (!Directory.Exists(parameters.WorkingDirectory))
                {
                    Directory.CreateDirectory(parameters.WorkingDirectory);
                }

                Status = RuntimeStatus.Initializing;
                WorkingDirectory = parameters.WorkingDirectory;
                CommandlineArgs = parameters.CommandlineArgs;

                SetupSerilog();

                m_Logger!.LogInformation("OpenMod v{Version} is starting...", Version);

                var hostInformationType = GetOpenmodHostInformation(openModHostAssemblies) ?? throw new Exception("Failed to find IHostInformation in host assemblies.");
                HostInformation = (IHostInformation)Activator.CreateInstance(hostInformationType);

                if (parameters.PackageManager is not NuGetPackageManager nugetPackageManager)
                {
                    var packagesDirectory = Path.Combine(WorkingDirectory, "packages");
                    nugetPackageManager = new NuGetPackageManager(packagesDirectory);
                }

                nugetPackageManager.Logger = new OpenModNuGetLogger(m_LoggerFactory!.CreateLogger("NuGet"));

                await nugetPackageManager.RemoveOutdatedPackagesAsync();
                nugetPackageManager.SetAssemblyLoader(Hotloader.LoadAssembly);

                var startupContext = new OpenModStartupContext
                {
                    Runtime = this,
                    LoggerFactory = m_LoggerFactory!,
                    NuGetPackageManager = nugetPackageManager,
                    DataStore = new Dictionary<string, object>()
                };

                var startup = new OpenModStartup(startupContext);
                startupContext.OpenModStartup = startup;

                foreach (var assembly in openModHostAssemblies)
                {
                    startup.RegisterIocAssemblyAndCopyResources(assembly, string.Empty);
                }

                var configFile = Path.Combine(WorkingDirectory, "openmod.yaml");
                if (File.Exists(configFile))
                {
                    var yaml = File.ReadAllText(configFile);
                    var deserializer = new DeserializerBuilder()
                        .WithTypeConverter(new YamlNullableEnumTypeConverter())
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    var config = deserializer.Deserialize<Dictionary<string, object>>(yaml);

                    var hotReloadingEnabled = true;
                    if (config.TryGetValue("hotreloading", out var unparsed))
                    {
                        switch (unparsed)
                        {
                            case bool value:
                                hotReloadingEnabled = value;
                                break;
                            case string strValue when bool.TryParse(strValue, out var parsed):
                                hotReloadingEnabled = parsed;
                                break;
                            default:
                                m_Logger!.LogWarning(
                                    "Unknown config for 'hotreloading' in OpenMod configuration: {UnparsedConfig}",
                                    unparsed);
                                break;
                        }
                    }
                    Hotloader.Enabled = hotReloadingEnabled;

                    var tryInstallMissingDependencies = false;
                    if (config.TryGetValue("nuget", out unparsed) && unparsed is Dictionary<object, object> nugetConfig)
                    {
                        if (nugetConfig.TryGetValue("tryAutoInstallMissingDependencies", out unparsed))
                        {
                            switch (unparsed)
                            {
                                case bool value:
                                    tryInstallMissingDependencies = value;
                                    break;
                                case string strValue when bool.TryParse(strValue, out var parsed):
                                    tryInstallMissingDependencies = parsed;
                                    break;
                                default:
                                    m_Logger!.LogWarning(
                                        "Unknown config for 'tryAutoInstallMissingDependencies' in OpenMod configuration: {UnparsedConfig}",
                                        unparsed);
                                    break;
                            }
                        }
                    }

                    PluginAssemblyStore.TryInstallMissingDependencies = tryInstallMissingDependencies;
                }

                await nugetPackageManager.InstallMissingPackagesAsync(updateExisting: true);
                await startup.LoadPluginAssembliesAsync();

                hostBuilder
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureHostConfiguration(builder =>
                    {
                        ConfigureConfiguration(builder, startup);
                    })
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        startup.Context.DataStore["HostBuilderContext"] = context;

                        ConfigureAppConfiguration(context, builder, startup);
                    })
                    .ConfigureContainer<ContainerBuilder>(builder => SetupContainer(builder, startup))
                    .ConfigureServices((_, services) =>
                    {
                        SetupServices(services, startup);
                    })
                    .UseSerilog((context, configuration) =>
                    {
                        SetupSerilog(configuration, context.Configuration);
                    });

                //TODO maybe add here dependencies missing msg
                //assembly.GetTypes() dont throw exception here
                //but after hostBuilder.Build() it does
                Host = hostBuilder.Build();

                m_LoggerFactory = Host.Services.GetRequiredService<ILoggerFactory>();
                m_Logger = m_LoggerFactory.CreateLogger<Runtime>();
                nugetPackageManager.Logger = new OpenModNuGetLogger(m_LoggerFactory.CreateLogger("NuGet"));

                var applicationLifetime = Host.Services.GetRequiredService<IHostApplicationLifetime>();
                applicationLifetime.ApplicationStopping.Register(() => { AsyncHelper.RunSync(ShutdownAsync); });

                Status = RuntimeStatus.Initialized;
                LifetimeScope = Host.Services.GetRequiredService<ILifetimeScope>().BeginLifetimeScopeEx(
                    containerBuilder =>
                    {
                        containerBuilder.Register(_ => this)
                            .As<IOpenModComponent>()
                            .SingleInstance()
                            .ExternallyOwned();

                        containerBuilder.RegisterType<ScopedPermissionChecker>()
                            .As<IPermissionChecker>()
                            .InstancePerLifetimeScope()
                            .OwnedByLifetimeScope();
                    });

                DataStore = Host.Services.GetRequiredService<IDataStoreFactory>().CreateDataStore(
                    new DataStoreCreationParameters
                    {
                        Component = this,
                        Prefix = "openmod",
                        Suffix = null,
                        WorkingDirectory = WorkingDirectory
                    });

                try
                {
                    await Host.StartAsync();
                }
                catch (Exception ex)
                {
                    Status = RuntimeStatus.Crashed;
                    m_Logger!.LogCritical(ex, "OpenMod has crashed");
                    Log.CloseAndFlush();
                }

                try
                {
                    PerformFileSystemWatcherPatch();
                }
                catch (Exception ex)
                {
                    m_Logger!.LogError(ex, "Failed to patch FileSystemWatcher");
                }

                return Host;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private Type? GetOpenmodHostInformation(IEnumerable<Assembly> openModHostAssemblies)
        {
            var missingOpenmodDepedencies = false;
            var assembliesTypes = new List<Type>();

            foreach (var openModHostAssembly in openModHostAssemblies)
            {
                try
                {
                    assembliesTypes.AddRange(openModHostAssembly.GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    assembliesTypes.AddRange(ex.Types.Where(t => t != null));

                    var missingDependencies = GetMissingDependencies(ex);
                    if (openModHostAssembly.GetName().Name.Equals("OpenMod.Unturned") && !missingDependencies.Any())
                        continue;

                    if (!missingOpenmodDepedencies)
                    {
                        missingOpenmodDepedencies = true;
                        m_Logger.LogWarning("Some OpenMod dependencies are missing, OpenMod could not work properly.");
                    }

                    m_Logger.LogDebug(ex, "Missing dependencies");
                    m_Logger.LogWarning("Some types from assembly {Assembly} couldn't be loaded.", openModHostAssembly.FullName);
                    m_Logger.LogWarning("Missing dependencies: {MissingAssemblies}", string.Join(", ", missingDependencies));
                }
            }

            return assembliesTypes.FirstOrDefault(t => t.IsAssignableTo<IHostInformation>());
        }

        private static readonly Regex s_MissingFileAssemblyVersionRegex =
            new("'(?<assembly>\\S+?), Version=(?<version>.+?), ",
                RegexOptions.Compiled);

        private static readonly Regex s_TypeLoadAssemblyVersionRegex =
            new("assembly:(?<assembly>\\S+?), Version=(?<version>.+?), ",
                RegexOptions.Compiled);
        //Copy of AssemblyExtensions.GetMissingDependencies (not perfect copy)
        private static ICollection<AssemblyName> GetMissingDependencies(ReflectionTypeLoadException reflectionTypeLoadException)
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
                var version = System.Version.Parse(match.Groups["version"].Value);

                if (assemblyName.Equals("Rocket.API") || missingAssemblies.TryGetValue(assemblyName, out var currentVersion) && currentVersion >= version)
                    continue;

                missingAssemblies[assemblyName] = version;
            }

            return missingAssemblies.Select(s => new AssemblyName(s.Key) { Version = s.Value }).ToList();
        }

        public async Task PerformSoftReloadAsync()
        {
            await ShutdownAsync();
            await InitAsync(m_OpenModHostAssemblies!, m_RuntimeInitParameters!, m_HostBuilderFunc);
        }

        private static readonly Type s_FileSytemWatcherType = typeof(FileSystemWatcher);

        private void PerformFileSystemWatcherPatch()
        {
            if (!RuntimeEnvironmentHelper.IsMono)
            {
                return;
            }

            if (RuntimeEnvironmentHelper.IsLinux)
            {
                using var tempFileSystemWatcher = new FileSystemWatcher();

                var internalWatcherField = s_FileSytemWatcherType
                    .GetField("watcher", BindingFlags.Instance | BindingFlags.NonPublic);

                if (internalWatcherField == null)
                {
                    m_Logger.LogWarning("Fail to obtain file system watcher.");
                    return;
                }

                var internalWatcher = internalWatcherField.GetValue(tempFileSystemWatcher);
                m_Logger.LogInformation("Using watcher: {FileWatcherImplementation}", internalWatcher.GetType().Name);
                return;
            }

            var defaultWatcherType = s_FileSytemWatcherType.Assembly
                .GetType("System.IO.DefaultWatcher");
            if (defaultWatcherType == null)
            {
                m_Logger.LogWarning("Fail to obtain default watcher type.");
                return;
            }

            var watcherInstanceField = defaultWatcherType.GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
            if (watcherInstanceField == null)
            {
                m_Logger.LogWarning("Fail to obtain watcher istance.");
                return;
            }

            var watcherInstance = watcherInstanceField.GetValue(null);
            var watcherType = watcherInstance.GetType();
            var watchesField = watcherType.GetField("watches", BindingFlags.Static | BindingFlags.NonPublic);

            if (watchesField == null)
            {
                m_Logger.LogWarning("Fail to obtain watches field.");
                return;
            }

            if (watchesField.GetValue(watcherInstance) is not Hashtable watches)
            {
                m_Logger.LogWarning("Watches is not Hashtable.");
                return;
            }

            var createFileDataMethod = watcherType.GetMethod("CreateFileData", BindingFlags.Static | BindingFlags.NonPublic);
            if (createFileDataMethod == null)
            {
                m_Logger.LogWarning("Watcher CreateFileData not found.");
                return;
            }

            lock (watches)
            {
                FieldInfo? incSubdirsField = null;
                FieldInfo? filesLockField = null;
                FieldInfo? filesField = null;

                // DictionaryEntry<FileSystemWatcher, DefaultWatcherData>
                foreach (DictionaryEntry entry in watches)
                {
                    if (entry.Key is not FileSystemWatcher fsw)
                    {
                        continue;
                    }

                    fsw.IncludeSubdirectories = false;

                    var watcherDataType = entry.Value.GetType();

                    incSubdirsField ??= watcherDataType.GetField("IncludeSubdirs", BindingFlags.Public | BindingFlags.Instance);
                    if (incSubdirsField?.GetValue(entry.Value) is false)//convert to bool and check if false
                    {
                        continue;
                    }

                    incSubdirsField?.SetValue(entry.Value, false);

                    filesLockField ??= watcherDataType.GetField("FilesLock", BindingFlags.Public | BindingFlags.Instance);
                    var @lock = filesLockField?.GetValue(entry.Value);

                    filesField ??= watcherDataType.GetField("Files", BindingFlags.Public | BindingFlags.Instance);
                    var files = filesField?.GetValue(entry.Value);

                    if (files is not Hashtable hashtable || @lock is null)
                    {
                        continue;
                    }

                    lock (@lock)
                    {
                        hashtable.Clear();

                        if (!fsw.Filter.Equals("*.*"))
                        {
                            continue;
                        }

                        // Re add file data to not call OnCreated event in FileSystemWatcher

                        var path = Path.GetFullPath(fsw.Path);
                        foreach (var fileName in Directory.GetFileSystemEntries(path, "*"))
                        {
                            hashtable.Add(fileName, createFileDataMethod.Invoke(null, new object[] { path, fileName }));
                        }
                    }
                }
            }
        }

        private void ConfigureConfiguration(IConfigurationBuilder builder, OpenModStartup startup)
        {
            builder
                .SetBasePath(WorkingDirectory)
                .AddYamlFile("openmod.yaml", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables("OpenMod_")
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { HostDefaults.ContentRootKey, WorkingDirectory },
                    { HostDefaults.ApplicationKey, "OpenMod" },
                });
            startup.ConfigureConfiguration(builder);
        }

        private void ConfigureAppConfiguration(HostBuilderContext? hostBuilderContext, IConfigurationBuilder builder, IOpenModStartup? startup = null)
        {
            var dateLogger = m_DateLogger ??= DateTime.Now;

            if (hostBuilderContext != null && startup?.Context is OpenModStartupContext context)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                context.Configuration = (IConfigurationRoot)hostBuilderContext.Configuration;
#pragma warning restore CS0618 // Type or member is obsolete
            }

            builder
                .SetBasePath(WorkingDirectory)
                .AddYamlFileEx(s =>
                {
                    s.Path = "logging.yaml";
                    s.Optional = false;
                    s.Variables = new Dictionary<string, string>
                    {
                        { "workingDirectory", WorkingDirectory.Replace(@"\", "/") },
                        { "date", dateLogger.ToString("yyyy-MM-dd-HH-mm-ss") }
                    };
                });
        }

        private void SetupServices(IServiceCollection services, OpenModStartup startup)
        {
            services.AddSingleton<IRuntime>(this);
            services.AddSingleton(HostInformation!);
            services.AddHostedService<OpenModHostedService>();
            services.AddOptions();
            services.AddSingleton(((OpenModStartupContext)startup.Context).NuGetPackageManager);
            startup.SetupServices(services);
        }

        private void SetupContainer(ContainerBuilder containerBuilder, OpenModStartup startup)
        {
            startup.SetupContainer(containerBuilder);
        }

        private void SetupSerilog(LoggerConfiguration? loggerConfiguration = null, IConfiguration? configuration = null)
        {
            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

            Log.CloseAndFlush();
            loggerConfiguration ??= new LoggerConfiguration();

            try
            {
                var loggingPath = Path.Combine(WorkingDirectory, "logging.yaml");
                if (!File.Exists(loggingPath))
                {
                    // First run, logging.yaml doesn't exist yet.
                    // We can't wait for auto-copy from the assembly as it would be too late.
                    using var stream =
                        typeof(AsyncHelper).Assembly.GetManifestResourceStream("OpenMod.Core.logging.yaml");
                    using var reader =
                        new StreamReader(
                            stream ?? throw new MissingManifestResourceException(
                                "Couldn't find resource: OpenMod.Core.logging.yaml"));

                    var fileContent = reader.ReadToEnd();
                    File.WriteAllText(loggingPath, fileContent);
                }

                if (configuration == null)
                {
                    var builder = new ConfigurationBuilder();
                    ConfigureAppConfiguration(null, builder);

                    configuration = builder.Build();
                }

                loggerConfiguration.ReadFrom.Configuration(configuration);
            }
            catch (Exception) when (m_LoggerFactory == null)
            {
                // If setting up first time logger then ignoring exception until it load NuGet packages
                // https://github.com/openmod/openmod/issues/341
                SetupDefaultLogger(loggerConfiguration);
            }
            catch (Exception ex)
            {
                SetupDefaultLogger(loggerConfiguration);

                var previousColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Failed to setup Serilog; logging will not work correctly.");
                Console.WriteLine("Setting up console only logging as workaround.");
                Console.WriteLine("Please fix your logging.yaml file or delete it to restore the default one.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);

                Console.ForegroundColor = previousColor;
            }

            // setup global log only for first time (before loading NuGet packages)
            if (m_LoggerFactory != null)
            {
                return;
            }

            Log.Logger = loggerConfiguration.CreateLogger();
            m_LoggerFactory = new SerilogLoggerFactory();
            m_Logger = m_LoggerFactory.CreateLogger<Runtime>();
        }

        private void SetupDefaultLogger(LoggerConfiguration loggerConfiguration)
        {
            const string defaultConsoleLogTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}";
            const string defaultFileLogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}";

            m_DateLogger ??= DateTime.Now;
            var logFilePath = $"{WorkingDirectory}/logs/openmod-{m_DateLogger:yyyy-MM-dd-HH-mm-ss}.log"
                .Replace(@"\", "/");

            loggerConfiguration
                .WriteTo.Async(c => c.Console(LogEventLevel.Information, defaultConsoleLogTemplate))
                .WriteTo.Async(c => c.File(logFilePath, LogEventLevel.Information, outputTemplate: defaultFileLogTemplate));
        }

        public async Task ShutdownAsync()
        {
            if (IsDisposing)
            {
                return;
            }

            IsDisposing = true;
            m_Logger.LogInformation("OpenMod is shutting down...");

            if (Host is not null)
            {
                await Host.DisposeSyncOrAsync();
                Host = null;
            }

            Status = RuntimeStatus.Unloaded;

            m_DateLogger = null;
            m_LoggerFactory = null;
            m_Logger = null;
            
            Log.CloseAndFlush();
        }
    }
}
