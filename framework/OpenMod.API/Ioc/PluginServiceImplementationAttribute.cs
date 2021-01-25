using System;
using JetBrains.Annotations;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// Declares a service implementation for automatic plugin scope IoC registratrion.
    /// Service implementations using this attribute are automatically registered for any interface that implements the <see cref="ServiceAttribute"/>.
    /// </summary>
    /// <seealso cref="ServiceAttribute"/>
    /// <seealso cref="ServiceImplementationAttribute"/>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PluginServiceImplementationAttribute : ServiceImplementationAttribute
    {
        
    }
}