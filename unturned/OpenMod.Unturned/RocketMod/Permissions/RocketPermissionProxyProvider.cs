using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.API.Permissions;
using OpenMod.Core.Helpers;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using UnturnedPlayer = Rocket.Unturned.Player.UnturnedPlayer;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class RocketPermissionProxyProvider : IRocketPermissionsProvider, IDisposable
    {
        private readonly IRocketModComponent m_RocketModComponent;
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly IPermissionRoleStore m_PermissionRoleStore;
        private readonly IPermissionRegistry m_PermissionRegistry;
        private IRocketPermissionsProvider? m_OriginalPermissionProvider;

        public RocketPermissionProxyProvider(
            IRocketModComponent rocketModComponent,
            IPermissionChecker permissionChecker,
            IPermissionRoleStore permissionRoleStore,
            IPermissionRegistry permissionRegistry)
        {
            m_RocketModComponent = rocketModComponent;
            m_PermissionChecker = permissionChecker;
            m_PermissionRoleStore = permissionRoleStore;
            m_PermissionRegistry = permissionRegistry;
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (player is UnturnedPlayer { IsAdmin: true })
            {
                return true;
            }

            if (requestedPermissions.Any(string.IsNullOrEmpty))
            {
                return true;
            }

            var actor = ConvertToActor(player);
            return AsyncHelper.RunSync(async () =>
            {
                foreach (var permission in requestedPermissions)
                {
                    var permissionRegistration = m_PermissionRegistry.FindPermission(m_RocketModComponent, permission);
                    if (permissionRegistration == null)
                    {
                        m_PermissionRegistry.RegisterPermission(m_RocketModComponent, permission);
                    }

                    if (await m_PermissionChecker.CheckPermissionAsync(actor, permission) == PermissionGrantResult.Grant)
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            var actor = ConvertToActor(player);
            var result = new List<RocketPermissionsGroup>();
            AsyncHelper.RunSync(async () =>
            {
                foreach (var group in await m_PermissionRoleStore.GetRolesAsync(actor))
                {
                    result.Add(GetGroup(group.Id)!);
                }
            });

            return result;
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            var actor = ConvertToActor(player);
            var result = new List<Permission>();
            AsyncHelper.RunSync(async () =>
            {
                foreach (var store in m_PermissionChecker.PermissionStores)
                {
                    var denied = await store.GetDeniedPermissionsAsync(actor);
                    result.AddRange(denied.Select(d => new Permission("!" + Normalize(d))));

                    var granted = await store.GetGrantedPermissionsAsync(actor);
                    result.AddRange(granted.Select(d => new Permission(Normalize(d))));
                }
            });
            return result;
        }

        private string Normalize(string permission)
        {
            return permission.Replace("RocketMod:", string.Empty);
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            return GetPermissions(player).Where(d => requestedPermissions.Any(e => e.Equals(d.Name, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (string.IsNullOrEmpty(groupId))
            {
                throw new ArgumentException(nameof(groupId));
            }

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
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (string.IsNullOrEmpty(groupId))
            {
                throw new ArgumentException(nameof(groupId));
            }

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

        public RocketPermissionsGroup? GetGroup(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                throw new ArgumentException(nameof(groupId));
            }

            return AsyncHelper.RunSync(async () =>
            {
                var group = await m_PermissionRoleStore.GetRoleAsync(groupId);
                if (@group == null)
                {
                    return null;
                }

                return new RocketPermissionsGroup(group.Id, group.DisplayName, null, new List<string>(), new List<Permission>());
            });
        }

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup @group)
        {
            if (@group == null)
            {
                throw new ArgumentNullException(nameof(@group));
            }

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
            if (@group == null)
            {
                throw new ArgumentNullException(nameof(@group));
            }

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
            if (string.IsNullOrEmpty(groupId))
            {
                throw new ArgumentException(nameof(groupId));
            }

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
            R.OnRockedInitialized += RocketModInitialized;
        }

        private void RocketModInitialized()
        {
            if (m_OriginalPermissionProvider == null && R.Permissions != this)
            {
                m_OriginalPermissionProvider = R.Permissions;
            }

            R.Permissions = this;
        }

        public void Dispose()
        {
            R.Permissions = m_OriginalPermissionProvider;
            R.OnRockedInitialized -= RocketModInitialized;
        }

        private IPermissionActor ConvertToActor(IRocketPlayer player)
        {
            return new OpenModPlayerPermissionActorWrapper(player);
        }

        private IPermissionRole ConvertToGroup(RocketPermissionsGroup group)
        {
            return new OpenModPermissionRoleWrapper(group);
        }
    }
}