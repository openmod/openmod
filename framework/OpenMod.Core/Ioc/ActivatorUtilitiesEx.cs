using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.Core.Ioc
{
    public static class ActivatorUtilitiesEx
    {
        /// <summary>
        /// Instantiate a type with constructor arguments provided directly and/or from an <see cref="IServiceProvider"/>.
        /// Ex: Instance will be owned by lifetime and auto-dispose when the lifetime gets disposed.
        /// </summary>
        /// <typeparam name="T">The type to activate</typeparam>
        /// <param name="lifetimeScope">The lifetime scope used to resolve dependencies</param>
        /// <param name="parameters">Constructor arguments not provided by the <paramref name="lifetimeScope"/>.</param>
        /// <returns>An activated object of type T</returns>
        public static T CreateInstance<T>(ILifetimeScope lifetimeScope, params object[] parameters)
        {
            return (T)CreateInstance(lifetimeScope, typeof(T), parameters);
        }

        /// <summary>
        /// Instantiate a type with constructor arguments provided directly and/or from an <see cref="IServiceProvider"/>.
        /// Ex: Instance will be owned by lifetime and auto-dispose when the lifetime gets disposed.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope used to resolve dependencies</param>
        /// <param name="instanceType">The type to activate</param>
        /// <param name="parameters">Constructor arguments not provided by the <paramref name="lifetimeScope"/>.</param>
        /// <returns>An activated object of type instanceType</returns>
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