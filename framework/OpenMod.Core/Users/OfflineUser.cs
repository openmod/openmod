using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    public class OfflineUser : UserBase
    {
        private readonly UserData m_Data;//todo

        public OfflineUser(IUserProvider userProvider, IUserDataStore userDataStore,  UserData data) : base(userProvider, userDataStore)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Id = data.Id ?? throw new InvalidOperationException("UserData.Id was null");
            Type = data.Type ?? throw new InvalidOperationException("UserData.Type was null");
            DisplayName = data.LastDisplayName ?? Id;
            m_Data = data;
        }

        public override Task PrintMessageAsync(string message)
        {
            // do nothing;
            return Task.CompletedTask;
        }

        public override Task PrintMessageAsync(string message, Color color)
        {
            // do nothing;
            return Task.CompletedTask;
        }
    }
}