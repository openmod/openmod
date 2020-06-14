using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Users
{
    [Service]
    public interface IUsersDataStore
    {
        List<UserData> Users { get; }
        Task ReloadAsync();
        Task SaveChangesAsync();
        Task<UserData> GetUserAsync(string userType, string userId);
    }
}