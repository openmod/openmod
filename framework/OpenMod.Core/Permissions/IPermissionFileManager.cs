using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Permissions
{
    [Service]
    public interface IPermissionFileManager
    {
        PermissionGroupsData PermissionGroupsData { get; }
        UsersData UsersData { get; }
        Task ReadPermissionGroupsAsync();
        Task ReadUsersDataAsync();
        Task SavePermissionGroupsAsync();
        Task SaveUsersAsync();
        Task<bool> PermissionGroupsDataExistsAsync();
        Task<bool> UsersDataExistsAsync();
    }
}