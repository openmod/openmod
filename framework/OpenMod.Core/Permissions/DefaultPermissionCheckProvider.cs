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

        public bool SupportsActor(IPermissionActor actor)
        {
            return true;
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

        
        private bool CheckPermissionEquals(string input, string permission)
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
            permission = permission.Replace(":", ".");

            var permissions = new List<string>
            {
                "*"
            };

            bool isFirst = true;
            var parentPath = string.Empty;
            foreach (var childPath in permission.Split('.'))
            {
                char seperator = isFirst ? ':' : '.';
                permissions.Add($"{parentPath}{childPath}{seperator}*");
                parentPath += $"{childPath}{seperator}";
                isFirst = false;
            }

            //remove last element because it should not contain "<permission>.*"
            //If someone has "permission.x.*" they should not have "permission.x" too
            permissions.RemoveAt(permissions.Count - 1);

            permissions.Add(permission);
            return permissions;
        }
    }
}