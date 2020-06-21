using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Permissions
{
    [Service]
    public interface IPermissionRolesDataStore
    {
        Task ReloadAsync();
        Task SaveChangesAsync();
        List<PermissionRoleData> Roles { get; }
        Task<PermissionRoleData> GetRoleAsync(string id);
    }
}