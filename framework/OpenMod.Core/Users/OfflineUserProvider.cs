using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    [Priority(Priority = Priority.Lowest)]
    public class OfflineUserProvider : IUserProvider
    {
        private readonly IUserDataStore m_UserDataStore;

        public OfflineUserProvider(IUserDataStore userDataStore)
        {
            m_UserDataStore = userDataStore;
        }
        public bool SupportsUserType(string userType)
        {
            return true;
        }

        public async Task<IUser?> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            if (searchMode != UserSearchMode.FindById && searchMode != UserSearchMode.FindByNameOrId)
            {
                return null;
            }

            var data = await m_UserDataStore.GetUserDataAsync(searchString, userType);
            if (data == null)
            {
                return null;
            }

            return new OfflineUser(this, m_UserDataStore, data);
        }

        public async Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType)
        {
            var userDatas = await m_UserDataStore.GetUsersDataAsync(userType);
            return userDatas.Select(d => new OfflineUser(this, m_UserDataStore, d)).ToList();
        }

        public Task BroadcastAsync(string userType, string message, Color? color)
        {
            return Task.CompletedTask;
        }

        public Task BroadcastWithIconAsync(string message, string? iconUrl = null, Color? color = null)
        {
            return Task.CompletedTask;
        }

        public Task BroadcastAsync(string message, Color? color)
        {
            return Task.CompletedTask;
        }

        public Task BroadcastWithIconAsync(string userType, string message, string? iconUrl = null, Color? color = null)
        {
            return Task.CompletedTask;
        }
    }
}