using System;
using JetBrains.Annotations;

namespace OpenMod.API.Ioc
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PluginServiceImplementationAttribute : ServiceImplementationAttribute
    {
        
    }
}