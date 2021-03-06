using System;
using Autofac;

namespace OpenMod.Core.Ioc
{
    public static class LifetimeScopeExtensions
    {
        /// <summary>
        /// Begin a new nested scope. Component instances created via the new scope
        /// will be disposed along with it. Ex: Will dispose if parent scope gets disposed.
        /// </summary>
        /// <returns>A new lifetime scope.</returns>
        public static ILifetimeScope BeginLifetimeScopeEx(this ILifetimeScope @this)
        {
            var scope = @this.BeginLifetimeScope();
            @this.Disposer.AddInstanceForAsyncDisposal(scope);
            return scope;
        }

        /// <summary>
        /// Begin a new sub-scope. Instances created via the sub-scope
        /// will be disposed along with it. Ex: Will dispose if parent scope gets disposed.
        /// </summary>
        /// <param name="this">The lifetime used to create the sub-scope</param>
        /// <param name="tag">The tag applied to the <see cref="ILifetimeScope"/>.</param>
        /// <returns>A new lifetime scope.</returns>
        public static ILifetimeScope BeginLifetimeScopeEx(this ILifetimeScope @this, object tag)
        {
            var scope = @this.BeginLifetimeScope(tag);
            @this.Disposer.AddInstanceForAsyncDisposal(scope);
            return scope;
        }

        /// <summary>
        /// Begin a new nested scope, with additional components available to it.
        /// Component instances created via the new scope
        /// will be disposed along with it. Ex: Will dispose if parent scope gets disposed.
        /// </summary>
        /// <param name="this">The lifetime used to create the sub-scope</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/>
        /// that adds component registrations visible only in the new scope.</param>
        /// <returns>A new lifetime scope.</returns>
        public static ILifetimeScope BeginLifetimeScopeEx(this ILifetimeScope @this, Action<ContainerBuilder> configurationAction)
        {
            var scope = @this.BeginLifetimeScope(configurationAction);
            @this.Disposer.AddInstanceForAsyncDisposal(scope);
            return scope;
        }

        /// <summary>
        /// Begin a new nested scope, with additional components available to it.
        /// Component instances created via the new scope
        /// will be disposed along with it. Ex: Will dispose if parent scope gets disposed.
        /// </summary>
        /// <param name="this">The lifetime used to create the sub-scope</param>
        /// <param name="tag">The tag applied to the <see cref="ILifetimeScope"/>.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/>
        /// that adds component registrations visible only in the new scope.</param>
        /// <returns>A new lifetime scope.</returns>
        public static ILifetimeScope BeginLifetimeScopeEx(this ILifetimeScope @this, object tag, Action<ContainerBuilder> configurationAction)
        {
            var scope = @this.BeginLifetimeScope(tag, configurationAction);
            @this.Disposer.AddInstanceForAsyncDisposal(scope);
            return scope;
        }
    }
}