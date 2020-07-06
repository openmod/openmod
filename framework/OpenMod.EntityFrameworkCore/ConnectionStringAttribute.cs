using System;

namespace OpenMod.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringAttribute : Attribute
    {
        public string Name { get; }

        public ConnectionStringAttribute(string name)
        {
            Name = name;
        }
    }
}