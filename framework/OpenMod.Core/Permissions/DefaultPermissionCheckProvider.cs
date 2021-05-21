using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Permissions
{
    [Priority(Priority = Priority.Lowest)]
    public class DefaultPermissionCheckProvider : IPermissionCheckProvider
    {
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly ILogger<DefaultPermissionCheckProvider> m_Logger;

        public DefaultPermissionCheckProvider(
            IPermissionChecker permissionChecker,
            ILogger<DefaultPermissionCheckProvider> logger)
        {
            m_PermissionChecker = permissionChecker;
            m_Logger = logger;
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


            m_Logger.LogDebug("Granted permissions for {ActorName}: ", actor.DisplayName);
            foreach (var knownPerm in grantedPermissions)
            {
                m_Logger.LogDebug("* {KnownPerm}", knownPerm);
            }

            m_Logger.LogDebug("Denied permissions for {ActorName}: ", actor.DisplayName);
            foreach (var knownPerm in deniedPermissions)
            {
                m_Logger.LogDebug("* {KnownPerm}", knownPerm);
            }


            var permissionTree = BuildPermissionTree(permission);
            foreach (var permissionNode in permissionTree)
            {
                m_Logger.LogDebug("Checking node: {PermissionNode}", permissionNode);

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
        ///   "OpenMod.Core:commands.help"
        /// </code>
        ///     <b>Example output:</b>
        ///     <code>
        /// [
        ///     "*",
        ///     "OpenMod.*"
        ///     "OpenMod.Core:*"
        ///     "OpenMod.Core:commands.*",
        ///     "OpenMod.Core:commands.help",
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

            var separatorIndices = permission.AllIndexesOf(":");

            // replace all ":" with "." for more simple code
            // we will restore : later again
            permission = permission.Replace(":", ".");

            var parentPath = new StringBuilder();
            foreach (var childPath in permission.Split('.'))
            {
                permissions.Add($"{parentPath}{childPath}.*");
                parentPath.Append(childPath);
                parentPath.Append(".");
            }

            // Remove last element because it should not contain "<permission>.*"
            // If someone has "permission.x.*" they should not have "permission.x" too
            permissions.RemoveAt(permissions.Count - 1);

            permissions.Add(permission);
            return permissions.Select(d =>
            {
                foreach (var index in separatorIndices)
                {
                    if (d.Length < index)
                    {
                        continue;
                    }

                    d = new StringBuilder(d) { [index] = ':' }.ToString();
                }

                return d;
            });
        }
    }
}