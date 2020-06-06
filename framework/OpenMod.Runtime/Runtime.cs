using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Util;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using OpenMod.API;
using OpenMod.Core.Helpers;
using Semver;
using Serilog;
using Serilog.Events;

namespace OpenMod.Runtime
{
    [UsedImplicitly]
    public sealed class Runtime : IRuntime
    {
        public string WorkingDirectory { get; private set; }
        private IHost m_Host;

        public void Init(ICollection<Assembly> openModAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters)
        {
            var thread = new Thread(o => { InitAsync(openModAssemblies, hostBuilder, parameters).Forget(); });
            thread.Start();
        }

        public async Task InitAsync(ICollection<Assembly> openModHostAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters)
        {
            SetupSerilog("logging.yml", parameters.WorkingDirectory, parameters.CommandlineArgs);
            Log.Information("OpenMod is starting...");

            try
            {

                if (!Directory.Exists(parameters.WorkingDirectory))
                {
                    Directory.CreateDirectory(parameters.WorkingDirectory);
                }

                WorkingDirectory = parameters.WorkingDirectory;
                
                hostBuilder
                    .UseContentRoot(parameters.WorkingDirectory)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureAppConfiguration(builder =>
                    {
                        SetupConfiguration(builder, parameters.CommandlineArgs, "openmod.yml", parameters.WorkingDirectory);
                    })
                    .ConfigureContainer<ContainerBuilder>(builder => SetupContainer(builder, openModHostAssemblies))
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<OpenModHostedService>();
                    })
                    .UseSerilog();

                m_Host = hostBuilder.Build();
                await m_Host.RunAsync();
                Log.Information("OpenMod has shut down.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "OpenMod has crashed.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private void SetupContainer(ContainerBuilder containerBuilder, ICollection<Assembly> openModHostAssemblies)
        {
            containerBuilder.Register(context => this)
                .As<IRuntime>()
                .SingleInstance();

            var registrator = new ContainerRegistrator(containerBuilder);

            // register from OpenMod.Core, should prob. use a different class than AsyncHelper
            registrator.RegisterServicesFromAssembly(typeof(AsyncHelper).Assembly);

            // register from assemblies such as OpenMod.UnityEngine, OpenMod.Unturned, etc.
            foreach (var assembly in openModHostAssemblies)
            {
                registrator.RegisterServicesFromAssembly(assembly);
            }

            registrator.BootstrapAndRegisterPlugins();
            registrator.CompleteRegistrations();
        }

        private void SetupSerilog(string serilogYaml, string basePath, string[] commandlineArgs)
        {
            var builder = new ConfigurationBuilder();
            SetupConfiguration(builder, commandlineArgs, serilogYaml, basePath);
            var configuration = builder.Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private void SetupConfiguration(IConfigurationBuilder configBuilder, string[] commandlineArgs, string file, string basePath)
        {
            configBuilder
                .SetBasePath(basePath)
                .AddYamlFile(file, true)
                .AddCommandLine(commandlineArgs)
                .AddEnvironmentVariables();
        }

        public async Task ShutdownAsync()
        {
            await m_Host.StopAsync();
        }

        public SemVersion Version { get; } = new SemVersion(0, 1, 0);
    }
}
