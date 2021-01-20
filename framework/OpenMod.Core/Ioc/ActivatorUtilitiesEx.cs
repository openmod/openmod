using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.Core.Ioc
{
    public static class ActivatorUtilitiesEx
    {
        public static T CreateInstance<T>(ILifetimeScope lifetimeScope, params object[] parameters)
        {
            return (T)CreateInstance(lifetimeScope, typeof(T), parameters);
        }

        public static object CreateInstance(ILifetimeScope lifetimeScope, Type instanceType, params object[] parameters)
        {
            var serviceProvider = lifetimeScope.Resolve<IServiceProvider>();
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, instanceType, parameters);

            if (instance is IAsyncDisposable asyncDisposable)
            {
                lifetimeScope.Disposer.AddInstanceForAsyncDisposal(asyncDisposable);
            }
            else if (instance is IDisposable disposable)
            {
                lifetimeScope.Disposer.AddInstanceForDisposal(disposable);
            }

            return instance;
        }
    }
}