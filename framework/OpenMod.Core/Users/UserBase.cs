using System;
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

        public virtual string Id { get; protected set; } = null!;

        public virtual string Type { get; protected set; } = null!;

        public virtual string DisplayName { get; protected set; } = null!;

        public virtual string FullActorName
        {
            get
            {
                return $"{DisplayName} ({Id})";
            }
        }

        public virtual IUserProvider? Provider { get; }

        public virtual IUserSession? Session { get; protected set; }

        public abstract Task PrintMessageAsync(string message);

        public abstract Task PrintMessageAsync(string message, Color color);

        public async Task SavePersistentDataAsync<T>(string key, T? data)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            var userData = await m_UserDataStore.GetUserDataAsync(Id, Type) ?? new UserData();
            userData.Data ??= new Dictionary<string, object?>();
            if (userData.Data.ContainsKey(key))
            {
                userData.Data[key] = data;
            }
            else
            {
                userData.Data.Add(key, data);
            }

            await m_UserDataStore.SetUserDataAsync(userData);
        }

        public Task<T?> GetPersistentDataAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            return m_UserDataStore.GetUserDataAsync<T>(Id, Type, key);
        }

        public override string ToString()
        {
            return $"{Type}/{DisplayName} ({Id})";
        }
    }
}