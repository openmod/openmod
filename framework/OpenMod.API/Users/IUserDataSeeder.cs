using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Service for seeding user data.
    /// </summary>
    [Service]
    public interface IUserDataSeeder
    {
        /// <summary>
        /// Seeds initial user data.
        /// </summary>
        Task SeedUserDataAsync(string actorId, string actorType, string? displayName, Dictionary<string, object?>? data = null);
    }
}