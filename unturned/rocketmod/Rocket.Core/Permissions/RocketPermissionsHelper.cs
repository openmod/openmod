using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core.Permissions
{
    internal class RocketPermissionsHelper
    {
        internal Asset<RocketPermissions> permissions;

        public RocketPermissionsHelper(Asset<RocketPermissions> permissions)
        {
            this.permissions = permissions;
        }

        public List<RocketPermissionsGroup> GetGroupsByIds(List<string> ids) => this.permissions.Instance.Groups.OrderBy(x => x.Priority)
            .Where(g => ids.Select(i => i.ToLower()).Contains(g.Id.ToLower())).ToList();

        public List<string> GetParentGroups(string parentGroup, string currentGroup)
        {
            var allGroups = new List<string>();
            RocketPermissionsGroup group = this.permissions.Instance.Groups.OrderBy(x => x.Priority)
                .FirstOrDefault(g => string.Equals(g.Id, parentGroup, StringComparison.CurrentCultureIgnoreCase));

            if (group == null || string.Equals(group.Id, currentGroup, StringComparison.CurrentCultureIgnoreCase)) { return allGroups; }

            allGroups.Add(group.Id);
            allGroups.AddRange(this.GetParentGroups(group.ParentGroup, currentGroup));

            return allGroups;
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            if (player.IsAdmin) { return true; }

            List<Permission> applyingPermissions = this.GetPermissions(player, requestedPermissions);

            return applyingPermissions.Count != 0;
        }

        internal RocketPermissionsGroup GetGroup(string groupId) => permissions.Instance.Groups.OrderBy(x => x.Priority)
            .FirstOrDefault(g => string.Equals(g.Id, groupId, StringComparison.CurrentCultureIgnoreCase));

        internal RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return RocketPermissionsProviderResult.GroupNotFound;

            if (g.Members.FirstOrDefault(m => m == player.Id) == null) return RocketPermissionsProviderResult.PlayerNotFound;

            g.Members.Remove(player.Id);
            SaveGroup(g);
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return RocketPermissionsProviderResult.GroupNotFound;

            if (g.Members.FirstOrDefault(m => m == player.Id) != null) return RocketPermissionsProviderResult.DuplicateEntry;

            g.Members.Add(player.Id);
            SaveGroup(g);
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            RocketPermissionsGroup g = GetGroup(groupId);
            if (g == null) return RocketPermissionsProviderResult.GroupNotFound;

            permissions.Instance.Groups.Remove(g);
            permissions.Save();
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i < 0) return RocketPermissionsProviderResult.GroupNotFound;
            permissions.Instance.Groups[i] = group;
            permissions.Save();
            return RocketPermissionsProviderResult.Success;
        }

        internal RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            int i = permissions.Instance.Groups.FindIndex(gr => gr.Id == group.Id);
            if (i != -1) return RocketPermissionsProviderResult.DuplicateEntry;
            permissions.Instance.Groups.Add(group);
            permissions.Save();
            return RocketPermissionsProviderResult.Success;
        }


        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            // get player groups
            List<RocketPermissionsGroup> groups = this.permissions.Instance?.Groups?.OrderBy(x => x.Priority)
                                                      .Where(g => g.Members.Contains(player.Id))
                                                      .ToList() ?? new List<RocketPermissionsGroup>();

            // get first default group
            RocketPermissionsGroup defaultGroup = this.permissions.Instance?.Groups?.OrderBy(x => x.Priority)
                .FirstOrDefault(g => string.Equals(g.Id, this.permissions.Instance.DefaultGroup, StringComparison.CurrentCultureIgnoreCase));

            // if exists, add to player groups
            if (defaultGroup != null) { groups.Add(defaultGroup); }

            // if requested, return list without parent groups
            if (!includeParentGroups) { return groups.Distinct().OrderBy(x => x.Priority).ToList(); }

            // add parent groups
            var parentGroups = new List<RocketPermissionsGroup>();
            groups.ForEach(g => parentGroups.AddRange(this.GetGroupsByIds(this.GetParentGroups(g.ParentGroup, g.Id))));
            groups.AddRange(parentGroups);

            return groups.Distinct().OrderBy(x => x.Priority).ToList();
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            var result = new List<Permission>();

            List<RocketPermissionsGroup> playerGroups = this.GetGroups(player, true);
            playerGroups.Reverse(); // because we need desc ordering

            playerGroups.ForEach(group =>
            {
                group.Permissions.ForEach(permission =>
                {

                    if (permission.Name.StartsWith("-"))
                    {
                        result.RemoveAll(x => string.Equals(x.Name, permission.Name.Substring(1), StringComparison.InvariantCultureIgnoreCase));
                    } 
                    else 
                    {
                        result.RemoveAll(x => x.Name == permission.Name);
                        result.Add(permission);
                    }

                });
            });

            return result.Distinct().ToList();
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            List<Permission> playerPermissions = this.GetPermissions(player);

            List<Permission> applyingPermissions = playerPermissions
                .Where(p => requestedPermissions.Exists(x => string.Equals(x, p.Name, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            if (playerPermissions.Exists(p => p.Name == "*")) { applyingPermissions.Add(new Permission("*")); }

            playerPermissions.ForEach(p =>
            {

                if (!p.Name.EndsWith(".*")) { return; }

                requestedPermissions.ForEach(requestedPermission =>
                {

                    int dotIndex = requestedPermission.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
                    string baseRequested = dotIndex > 0 ? requestedPermission.Substring(0, dotIndex) : requestedPermission;

                    dotIndex = p.Name.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
                    string basePlayer = dotIndex > 0 ? p.Name.Substring(0, dotIndex) : p.Name;

                    if (string.Equals(basePlayer, baseRequested, StringComparison.InvariantCultureIgnoreCase)) { applyingPermissions.Add(p); }

                });

            });

            return applyingPermissions.Distinct().ToList();
        }

    }

}