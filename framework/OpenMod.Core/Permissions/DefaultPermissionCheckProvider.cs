using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Permissions
{
    [Priority(Priority = Priority.Lowest)]
    public class DefaultPermissionCheckProvider : IPermissionCheckProvider
    {
        private readonly IPermissionChecker m_PermissionChecker;

        public DefaultPermissionCheckProvider(IPermissionChecker permissionChecker)
        {
            m_PermissionChecker = permissionChecker;
        }

        public bool SupportsActor(object actor)
        {
            return actor is IPermissionActor;
        }

        public async Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            var grantedPermissions = new List<string>();
            var deniedPermissions = new List<string>();

            foreach (var permissionSource in m_PermissionChecker.PermissionStores)
            {
                grantedPermissions.AddRange(await permissionSource.GetGrantedPermissionsAsync(actor));
                deniedPermissions.AddRange(await permissionSource.GetDeniedPermissionsAsync(actor));
            }

            var permissionTree = BuildPermissionTree(permission);
            foreach (var permissionNode in permissionTree)
            {
                if (deniedPermissions.Any(c => CheckPermissionEquals(permissionNode, c)))
                {
                    return PermissionGrantResult.Deny;
                }

                if (grantedPermissions.Any(c => CheckPermissionEquals(permissionNode, c)))
                {
                    return PermissionGrantResult.Grant;
                }
            }

            return PermissionGrantResult.Default;
        }

        private static bool CheckPermissionEquals(string input, string permission)
        {
            return input.Equals(permission, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Builds a parent permission tree for the given permission <br />
        ///     If the target has any of these permissions, they will automatically have the given permission too <br /><br />
        ///     <b>Example Input:</b>
        /// <code>
        ///   "player.test.sub"
        /// </code>
        ///     <b>Example output:</b>
        ///     <code>
        /// [
        ///     "*",
        ///     "player.*",
        ///     "player.test.*",
        ///     "player.test.sub"
        /// ]
        /// </code>
        /// </summary>
        /// <param name="permission">The permission to build the tree for</param>
        /// <returns>The collection of all parent permission nodes</returns>
        public static IEnumerable<string> BuildPermissionTree(string permission)
        {
            var permissions = new List<string>
            {
                "*"
            };


            var parentPath = string.Empty;
            foreach (var childPath in permission.Split('.'))
            {
                permissions.Add(parentPath + childPath + ".*");
                parentPath += childPath + ".";
            }

            //remove last element because it should not contain "<permission>.*"
            //If someone has "permission.x.*" they should not have "permission.x" too
            permissions.RemoveAt(permissions.Count - 1);

            permissions.Add(permission);
            return permissions;
        }
    }
}