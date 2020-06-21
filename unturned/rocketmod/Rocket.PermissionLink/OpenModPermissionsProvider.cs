using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.API.Permissions;
using OpenMod.Core.Helpers;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;

namespace Rocket.PermissionLink
{
    public class OpenModPermissionsProvider : IRocketPermissionsProvider, IDisposable
    {
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly IPermissionRoleStore m_PermissionRoleStore;
        private IRocketPermissionsProvider m_OriginalPermissionProvider;

        public OpenModPermissionsProvider(
            IPermissionChecker permissionChecker,
            IPermissionRoleStore permissionRoleStore)
        {
            m_PermissionChecker = permissionChecker;
            m_PermissionRoleStore = permissionRoleStore;
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            var actor = ConvertToActor(player);
            return AsyncHelper.RunSync(async () =>
            {
                foreach (var permission in requestedPermissions)
                {
                    if (await m_PermissionChecker.CheckPermissionAsync(actor, permission) != PermissionGrantResult.Grant)
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            var actor = ConvertToActor(player);
            var result = new List<RocketPermissionsGroup>();
            AsyncHelper.RunSync(async () =>
            {
                foreach (var group in await m_PermissionRoleStore.GetRolesAsync(actor))
                {
                    result.Add(GetGroup(group.Id));
                }
            });

            return result;
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            var actor = ConvertToActor(player);
            var result = new List<Permission>();
            AsyncHelper.RunSync(async () =>
            {
                foreach (var store in m_PermissionChecker.PermissionStores)
                {
                    var denied = await store.GetDeniedPermissionsAsync(actor);
                    result.AddRange(denied.Select(d => new Permission("!" + d)));

                    var granted = await store.GetGrantedPermissionsAsync(actor);
                    result.AddRange(granted.Select(d => new Permission(d)));
                }
            });
            return result;
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            return GetPermissions(player).Where(d => requestedPermissions.Any(e => e.Equals(d.Name, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            var actor = ConvertToActor(player);
            return AsyncHelper.RunSync(async () =>
            {
                if (await m_PermissionRoleStore.AddRoleToActorAsync(actor, groupId))
                {
                    return RocketPermissionsProviderResult.Success;
                }

                return RocketPermissionsProviderResult.UnspecifiedError;
            });
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            var actor = ConvertToActor(player);
            return AsyncHelper.RunSync(async () =>
            {
                if (await m_PermissionRoleStore.RemoveRoleFromActorAsync(actor, groupId))
                {
                    return RocketPermissionsProviderResult.Success;
                }

                return RocketPermissionsProviderResult.UnspecifiedError;
            });
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return AsyncHelper.RunSync(async () =>
            {
                var group = await m_PermissionRoleStore.GetRoleAsync(groupId);
                return new RocketPermissionsGroup(group.Id, group.DisplayName, null, new List<string>(),  new List<Permission>());
            });
        }

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup @group)
        {
            var permissionGroup = ConvertToGroup(@group);
            return AsyncHelper.RunSync(async () =>
            {
                if (await m_PermissionRoleStore.CreateRoleAsync(permissionGroup))
                {
                    return RocketPermissionsProviderResult.Success;
                }

                return RocketPermissionsProviderResult.UnspecifiedError;
            });
        }

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup @group)
        {
            var permissionGroup = ConvertToGroup(@group);
            return AsyncHelper.RunSync(async () =>
            {
                if (await m_PermissionRoleStore.UpdateRoleAsync(permissionGroup))
                {
                    return RocketPermissionsProviderResult.Success;
                }

                return RocketPermissionsProviderResult.UnspecifiedError;
            });
        }

        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            return AsyncHelper.RunSync(async () =>
            {
                if (await m_PermissionRoleStore.DeleteRoleAsync(groupId))
                {
                    return RocketPermissionsProviderResult.Success;
                }

                return RocketPermissionsProviderResult.UnspecifiedError;
            });
        }

        public void Reload()
        {
            // do nothing
        }

        public void Install()
        {
            m_OriginalPermissionProvider = R.Permissions;
            R.Permissions = this;
        }

        public void Dispose()
        {
            R.Permissions = m_OriginalPermissionProvider;
        }

        private IPermissionActor ConvertToActor(IRocketPlayer player)
        {
            return new RocketPlayerPermissionActorWrapper(player);
        }

        private IPermissionRole ConvertToGroup(RocketPermissionsGroup group)
        {
            return new RocketPermissionRoleWrapper(group);
        }
    }
}