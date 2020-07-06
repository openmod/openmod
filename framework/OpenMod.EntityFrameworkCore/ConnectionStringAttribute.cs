using System;

namespace OpenMod.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringAttribute : Attribute
    {
        public string Name { get; }

        public ConnectionStringAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Connectiong string name can not be null or empty", nameof(name));
            }

            Name = name;
        }
    }
}