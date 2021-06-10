using Autofac;

namespace OpenMod.EntityFrameworkCore.MySql.Extensions
{
    public static class EntityFrameworkCoreMySqlContainerBuilderExtensions
    {
        public static ContainerBuilder AddEntityFrameworkCoreMySql(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ConfigurationBasedConnectionStringAccessor>()
                .As<ConfigurationBasedConnectionStringAccessor>()
                .As<IConnectionStringAccessor>()
                .OwnedByLifetimeScope()
                .InstancePerDependency();

            containerBuilder.RegisterType<PomeloMySqlConnectorResolver>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();

            return containerBuilder;
        }
    }
}
