using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Users
{
    [Service]
    public interface IUserDataStore
    {
        Task<UserData> GetUserDataAsync(string userId, string userType);
        Task<T> GetUserDataAsync<T>(string userId, string userType, string key);
        Task<IReadOnlyCollection<UserData>> GetUsersDataAsync(string type);
        Task SaveUserDataAsync(UserData userData);
    }
}