using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Users;

namespace OpenMod.Core.Permissions
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class DefaultPermissionGroupStore : IPermissionGroupStore
    {
        private readonly IPermissionGroupsDataStore m_PermissionGroupsDataStore;
        private readonly IUsersDataStore m_UsersDataStore;
        public DefaultPermissionGroupStore(
            IPermissionGroupsDataStore permissionGroupsDataStore, 
            IUsersDataStore usersDataStore)
        {
            m_PermissionGroupsDataStore = permissionGroupsDataStore;
            m_UsersDataStore = usersDataStore;
        }
        
        public virtual async Task<IReadOnlyCollection<IPermissionGroup>> GetGroupsAsync(IPermissionActor actor, bool inherit = true)
        {
            var groups = new List<IPermissionGroup>();
            var groupsIds = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            if (actor is IPermissionGroup group)
            {
                groups.Add(group);
                groupsIds.Add(group.Id);

                if (!inherit)
                    return groups;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var parentGroupId in group.Parents)
                {
                    if (groupsIds.Contains(parentGroupId))
                        continue;

                    var parentGroup = await GetGroupAsync(parentGroupId);
                    if (parentGroup == null) 
                        continue;

                    groups.Add(parentGroup);
                    groupsIds.Add(parentGroupId);
                }

                return groups;
            }

            var user = await GetOrCreateUserDataAsync(actor);
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var groupId in user.Groups)
            {
                if (groupsIds.Contains(groupId)) //prevent add group that was already by parent for example
                    continue;
                
                var userGroup = await GetGroupAsync(groupId);
                if (userGroup == null)
                    continue;

                groups.Add(userGroup);
                groupsIds.Add(groupId);

                if (!inherit)
                    continue;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var parentGroupId in userGroup.Parents)
                {
                    if (groupsIds.Contains(parentGroupId))
                        continue;

                    var parentGroup = await GetGroupAsync(parentGroupId);
                    if (parentGroup == null) 
                        continue;

                    groups.Add(parentGroup);
                    groupsIds.Add(parentGroupId);
                }
            }

            return groups;
        }

        public virtual async Task AssignAutoGroupsToUserAsync(IPermissionActor actor)
        {
            if (actor is IPermissionGroup)
            {
                throw new Exception("Can not auto assignment on permission groups.");
            }

            var user = m_UsersDataStore.Users.FirstOrDefault(d => d.Id.Equals(actor.Id, StringComparison.InvariantCultureIgnoreCase));
            if (user == null)
            {
                // CreateUser will assign groups itself
                await GetOrCreateUserDataAsync(actor);
                return;
            }

            foreach (var group in GetAutoAssignPermissionGroups().Where(gp => !user.Groups.Contains(gp.Id)))
            {
                user.Groups.Add(group.Id);
            }

            await m_UsersDataStore.SaveChangesAsync();
        }

        protected virtual async Task<UserData> GetOrCreateUserDataAsync(IPermissionActor actor)
        {
            var user = m_UsersDataStore.Users.FirstOrDefault(d => d.Id == actor.Id);
            if (user != null)
            {
                return user;
            }

            var userData = new UserData
            {
                Id = actor.Id,
                Type = actor.Type,
                FirstSeen = DateTime.Now,
                LastDisplayName = actor.DisplayName,
                LastSeen = DateTime.Now,
                Groups = new HashSet<string>(
                    GetAutoAssignPermissionGroups().Select(d => d.Id), 
                    StringComparer.InvariantCultureIgnoreCase)
            };

            m_UsersDataStore.Users.Add(userData);
            await m_UsersDataStore.SaveChangesAsync();
            return userData;
        }

        public virtual Task<IReadOnlyCollection<IPermissionGroup>> GetGroupsAsync()
        {
            //cast is neccessary, OfType<> or Cast<> does not work with cast operators
            return Task.FromResult((IReadOnlyCollection<IPermissionGroup>)m_PermissionGroupsDataStore.PermissionGroups
                .Select(d => (IPermissionGroup)(PermissionGroup)d)
                .ToList());
        }

        public virtual Task<IPermissionGroup> GetGroupAsync(string id)
        {
            //cast is neccessary, OfType<> or Cast<> does not work with cast operators
            return Task.FromResult(m_PermissionGroupsDataStore.PermissionGroups
                .Select(d => (IPermissionGroup)(PermissionGroup)d)
                .FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual async Task<bool> UpdateGroupAsync(IPermissionGroup group)
        {
            if (await GetGroupAsync(group.Id) == null)
            {
                return false;
            }

            var groupData = m_PermissionGroupsDataStore.PermissionGroups.First(d => d.Id.EndsWith(group.Id));
            groupData.DisplayName = group.DisplayName;
            groupData.IsAutoAssigned = group.IsAutoAssigned;
            groupData.Parents = group.Parents;
            groupData.Permissions = group.Parents;
            groupData.Priority = group.Priority;
            groupData.Data ??= new Dictionary<string, object>();

            await m_PermissionGroupsDataStore.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> AddGroupToActorAsync(IPermissionActor actor, string groupId)
        {
            var user = await GetOrCreateUserDataAsync(actor);
            if (user.Groups.Contains(groupId))
            {
                return true;
            }

            user.Groups.Add(groupId);
            await m_UsersDataStore.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> RemoveGroupFromActorAsync(IPermissionActor actor, string groupId)
        {
            var user = await GetOrCreateUserDataAsync(actor);
            user.Groups.Remove(groupId);

            await m_UsersDataStore.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> CreateGroupAsync(IPermissionGroup group)
        {
            if (await GetGroupAsync(group.Id) != null)
            {
                return false;
            }

            m_PermissionGroupsDataStore.PermissionGroups.Add(new PermissionGroupData
            {
                Priority = group.Priority,
                Id = group.Id,
                DisplayName = group.DisplayName,
                Parents = new HashSet<string>(group.Parents, StringComparer.InvariantCultureIgnoreCase),
                Permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase),
                IsAutoAssigned = group.IsAutoAssigned
            });

            await m_PermissionGroupsDataStore.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> DeleteGroupAsync(string groupId)
        {
            if (m_PermissionGroupsDataStore.PermissionGroups.RemoveAll(c => groupId.Equals(c.Id, StringComparison.OrdinalIgnoreCase)) == 0)
            {
                return false;
            }

            await m_PermissionGroupsDataStore.SaveChangesAsync();
            return true;
        }

        protected IEnumerable<IPermissionGroup> GetAutoAssignPermissionGroups()
        {
            //cast is neccessary, OfType<> or Cast<> does not work with cast operators
            return m_PermissionGroupsDataStore.PermissionGroups
                .Select(d => (PermissionGroup)d)
                .Where(d => d.IsAutoAssigned);
        }
    }
}