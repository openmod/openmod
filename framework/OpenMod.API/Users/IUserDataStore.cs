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
        Task SetUserDataAsync<T>(string userId, string userType, string key, T value);
        Task<IReadOnlyCollection<UserData>> GetUsersDataAsync(string type);
        Task SetUserDataAsync(UserData userData);
    }
}