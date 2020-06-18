using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Users;

namespace OpenMod.Core.Permissions
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class DefaultPermissionGroupStore : IPermissionGroupStore
    {
        private readonly IPermissionGroupsDataStore m_PermissionGroupsDataStore;
        private readonly IUserDataStore m_UserDataStore;

        public DefaultPermissionGroupStore(IPermissionGroupsDataStore permissionGroupsDataStore, IUserDataStore userDataStore)
        {
            m_PermissionGroupsDataStore = permissionGroupsDataStore;
            m_UserDataStore = userDataStore;
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

            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var groupId in userData.Groups)
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
            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            if (userData.Groups.Contains(groupId))
            {
                return true;
            }

            userData.Groups.Add(groupId);
            await m_UserDataStore.SaveUserDataAsync(userData);
            return true;
        }

        public virtual async Task<bool> RemoveGroupFromActorAsync(IPermissionActor actor, string groupId)
        {
            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            userData.Groups.Remove(groupId);
            await m_UserDataStore.SaveUserDataAsync(userData);
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

        public Task<IReadOnlyCollection<string>> GetAssignAutoGroupsAsync(string actorId, string actorType)
        {
            IReadOnlyCollection<string> result =  GetAutoAssignPermissionGroups().Select(d => d.Id).ToList();
            return Task.FromResult(result);
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