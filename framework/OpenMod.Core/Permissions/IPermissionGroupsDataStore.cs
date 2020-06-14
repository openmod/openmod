using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Permissions
{
    [Service]
    public interface IPermissionGroupsDataStore
    {
        Task ReloadAsync();
        Task SaveChangesAsync();
        List<PermissionGroupData> PermissionGroups { get; }
        Task<PermissionGroupData> GetGroupAsync(string id);
    }
}