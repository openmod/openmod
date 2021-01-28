using System;

namespace OpenMod.EntityFrameworkCore
{
    /// <summary>
    /// Sets the connection string to use for a <see cref="OpenModDbContext{TSelf}"/>. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringAttribute : Attribute
    {
        /// <summary>
        /// The connection string name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Sets the connection string to use for a<see cref="OpenModDbContext{TSelf}"/>.
        /// </summary>
        /// <param name="name">The connection string name.</param>
        public ConnectionStringAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Connection string name cannot be null or empty.", nameof(name));
            }

            Name = name;
        }
    }
}