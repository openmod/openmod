using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace OpenMod.API.Users
{
    public interface IUserProvider
    {
        bool SupportsUserType(string userType);

        Task<IUser> FindUserAsync(string userType, string searchString, UserSearchMode searchMode);

        Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType);

        Task BroadcastAsync(string userType, string message, Color color);
        
        Task BroadcastAsync(string message, Color color);
    }
}