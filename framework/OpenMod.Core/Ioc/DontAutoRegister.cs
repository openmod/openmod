using System;

namespace OpenMod.Core.Ioc
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DontAutoRegister : Attribute
    {
        
    }
}