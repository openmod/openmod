using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.API.Users
{
    public interface IUserProvider
    {
        bool SupportsUserType(string userType);

        Task<IUser> FindUserAsync(string userType, string searchString, UserSearchMode searchMode);

        Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType);
    }
}