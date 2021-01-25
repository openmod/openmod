using System;
using JetBrains.Annotations;

namespace OpenMod.API.Ioc
{
    /// <summary>
    /// Declares the interfaces as a service for automatic IoC registration.
    /// <seealso cref="ServiceImplementationAttribute"/>
    /// <seealso cref="PluginServiceImplementationAttribute"/>
    /// </summary>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute
    {
    }
}