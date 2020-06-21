using System;
using System.Collections.Generic;
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

        public async Task<IUser> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            if (searchMode != UserSearchMode.Id && searchMode != UserSearchMode.NameOrId)
            {
                return null;
            }

            var data = await m_UserDataStore.GetUserDataAsync(searchString, userType);
            return new OfflineUser(m_UserDataStore, data);
        }

        public async Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType)
        {
            var userDatas = await m_UserDataStore.GetUsersDataAsync(userType);
            return userDatas.Select(d => new OfflineUser(m_UserDataStore, d)).ToList();
        }
    }
}