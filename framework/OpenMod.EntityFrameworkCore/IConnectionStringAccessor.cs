using OpenMod.API.Ioc;

namespace OpenMod.EntityFrameworkCore
{
    /// <summary>
    /// The service for resolving connection strings.
    /// </summary>
    [Service]
    public interface IConnectionStringAccessor
    {
        /// <summary>
        /// Gets a connection string.
        /// </summary>
        /// <param name="name">The name of the connection string.</param>
        /// <returns><b>The connection string</b> if found; otherwise, <b>null</b>.</returns>
        string? GetConnectionString(string name);
    }
}