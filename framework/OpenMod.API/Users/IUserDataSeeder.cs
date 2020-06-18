using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Users
{
    [Service]
    public interface IUserDataSeeder
    {
        Task SeedUserDataAsync(string actorId, string actorType, string displayName, Dictionary<string, object> data = null);
    }
}