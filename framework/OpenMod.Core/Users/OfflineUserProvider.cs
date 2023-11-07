using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    [Priority(Priority = Priority.Lowest)]
    public class OfflineUserProvider : IUserProvider
    {
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private readonly IUserDataStore m_UserDataStore;

        public OfflineUserProvider(IOpenModStringLocalizer stringLocalizer, IUserDataStore userDataStore)
        {
            m_StringLocalizer = stringLocalizer;
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

        public Task BroadcastAsync(string message, Color? color)
        {
            return Task.CompletedTask;
        }

        public Task<bool> BanAsync(IUser user, string? reason = null, DateTime? endTime = null)
        {
            return BanAsync(user, instigator: null, reason, endTime);
        }

        public async Task<bool> BanAsync(IUser user, IUser? instigator = null, string? reason = null, DateTime? expireDate = null)
        {
            if (expireDate.HasValue && expireDate.Value < DateTime.Now)
                return false;

            var data = await m_UserDataStore.GetUserDataAsync(user.Id, user.Type);
            if (data == null)
            {
                return false;
            }

            expireDate ??= DateTime.MaxValue;
            reason ??= m_StringLocalizer["ban_default"];
            data.BanInfo = new BanData(reason!, instigator, expireDate);

            await m_UserDataStore.SetUserDataAsync(data);
            return true;
        }

        public Task<bool> KickAsync(IUser user, string? reason = null)
        {
            return Task.FromResult(result: false);
        }
    }
}
