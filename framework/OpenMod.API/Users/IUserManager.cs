using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Users
{
    [Service]
    public interface IUserManager
    {
        IReadOnlyCollection<IUserProvider> UserProviders { get; }

        Task<IReadOnlyCollection<IUser>> GetUsers(string type);

        Task<IUser> FindUserAsync(string type, string searchString, UserSearchMode searchMode);
    }
}