using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    public class OfflineUser : UserBase
    {
        private readonly UserData m_Data;

        public OfflineUser(IUserDataStore userDataStore,  UserData data) : base(userDataStore)
        {
            Id = data.Id;
            Type = data.Type;
            DisplayName = data.LastDisplayName;
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