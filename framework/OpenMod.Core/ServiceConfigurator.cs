using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Commands;
using OpenMod.Core.Commands.Parameters;
using OpenMod.Core.Console;
using OpenMod.Core.Localization;
using OpenMod.Core.Permissions;
using OpenMod.Core.Plugins;
using OpenMod.Core.Users;
using System;
using OpenMod.Core.Jobs;
using SmartFormat.Extensions;
using OpenMod.Core.Persistence;

namespace OpenMod.Core
{
    [UsedImplicitly]
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<PermissionCheckerOptions>(options =>
            {
                options.AddPermissionCheckProvider<DefaultPermissionCheckProvider>();
                options.AddPermissionCheckProvider<ConsolePermissionProvider>();
                options.AddPermissionSource<DefaultPermissionStore>();
            });

            serviceCollection.Configure<CommandStoreOptions>(options =>
            {
                var logger = openModStartupContext.LoggerFactory.CreateLogger<OpenModComponentCommandSource>();
                options.AddCommandSource(new OpenModComponentCommandSource(logger, openModStartupContext.Runtime, GetType().Assembly));
            });

            serviceCollection.Configure<UserManagerOptions>(options =>
            {
                options.AddUserProvider<OfflineUserProvider>();
            });

            serviceCollection.Configure<CommandParameterResolverOptions>(options =>
            {
                options.AddCommandParameterResolveProvider<TypeDescriptorCommandParameterResolveProvider>();
                options.AddCommandParameterResolveProvider<UserCommandParameterResolveProvider>();
                options.AddCommandParameterResolveProvider<TimeSpanCommandParameterResolveProvider>();
            });

            serviceCollection.Configure<JobExecutorOptions>(options =>
            {
                options.AddJobExecutor<OpenModCommandTaskExecutor>();
                options.AddJobExecutor<SystemCommandTaskExecutor>();
            });

            serviceCollection.Configure<SmartFormatOptions>(options =>
            {
                options.TryAddFormatter<TimeFormatter>();
            });

            serviceCollection.Configure<YamlDataStoreOptions>(options =>
            {
                options.TryAddConverter<YamlNullableEnumTypeConverter>();
                options.TryAddConverter<YamlVector3TypeConverter>();
            });

            serviceCollection.AddTransient<IStringLocalizerFactory, ConfigurationBasedStringLocalizerFactory>();
            serviceCollection.AddTransient(typeof(IPluginAccessor<>), typeof(PluginAccessor<>));
            serviceCollection.AddSingleton<IAutoCompleteHandler, CommandAutoCompleteHandler>();

            serviceCollection.AddHostedService<UsersSchemaGenerator>();
        }
    }
}