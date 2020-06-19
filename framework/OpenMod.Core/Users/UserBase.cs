using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Users
{
    public abstract class UserBase : IUser
    {
        private readonly IUserDataStore m_UserDataStore;

        protected UserBase(IUserDataStore userDataStore)
        {
            m_UserDataStore = userDataStore;
        }

        public virtual string Id { get; protected set; }

        public virtual string Type { get; protected set; }

        public virtual string DisplayName { get; protected set; }

        public virtual IUserSession Session { get; protected set; }

        public abstract Task PrintMessageAsync(string message);

        public abstract Task PrintMessageAsync(string message, Color color);

        public async Task SavePersistentDataAsync<T>(string key, T data) where T : class
        {
            var userData = await m_UserDataStore.GetUserDataAsync(Id, Type);
            userData.Data ??= new Dictionary<string, object>();
            userData.Data.Add(key, data);
            await m_UserDataStore.SaveUserDataAsync(userData);
        }

        public async Task<T> GetPersistentDataAsync<T>(string key) where T : class
        {
            var data = await m_UserDataStore.GetUserDataAsync(Id, Type);
            if (!data.Data.ContainsKey(key))
            {
                return null;
            }

            var dataObject = data.Data[key];

            if (dataObject is T obj)
            {
                return obj;
            }

            if (dataObject.GetType().HasConversionOperator(typeof(T)))
            {
                // ReSharper disable once PossibleInvalidCastException
                return (T) dataObject;
            }

            if (dataObject is Dictionary<string, object> dict)
            {
                return dict.ToObject<T>();
            }

            throw new Exception($"Failed to parse {dataObject.GetType()} as {typeof(T)}");
        }
    }
}