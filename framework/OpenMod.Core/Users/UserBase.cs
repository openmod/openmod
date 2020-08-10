using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    public abstract class UserBase : IUser
    {
        private readonly IUserDataStore m_UserDataStore;

        protected UserBase(IUserProvider userProvider, IUserDataStore userDataStore)
        {
            Provider = userProvider;
            m_UserDataStore = userDataStore;
        }

        public virtual string Id { get; protected set; }
        
        public virtual IUserProvider Provider { get; }

        public virtual string Type { get; protected set; }

        public virtual string DisplayName { get; protected set; }

        public virtual IUserSession Session { get; protected set; }

        public abstract Task PrintMessageAsync(string message);

        public abstract Task PrintMessageAsync(string message, Color color);

        public async Task SavePersistentDataAsync<T>(string key, T data)
        {
            var userData = await m_UserDataStore.GetUserDataAsync(Id, Type);
            userData.Data ??= new Dictionary<string, object>();
            if (userData.Data.ContainsKey(key))
            {
                userData.Data[key] = data;
            }
            else
            {
                userData.Data.Add(key, data);
            }

            await m_UserDataStore.SaveUserDataAsync(userData);
        }

        public async Task<T> GetPersistentDataAsync<T>(string key)
        {
            return await m_UserDataStore.GetUserDataAsync<T>(Id, Type, key);
        }
    }
}