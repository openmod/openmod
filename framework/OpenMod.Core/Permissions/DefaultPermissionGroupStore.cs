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

namespace OpenMod.Core.Permissions
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class DefaultPermissionGroupStore : IPermissionGroupStore
    {
        private readonly IPermissionFileManager m_PermissionFileManager;
        public DefaultPermissionGroupStore(IPermissionFileManager permissionFileManager)
        {
            m_PermissionFileManager = permissionFileManager;
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

            var user = m_PermissionFileManager.UsersData.Users.FirstOrDefault(d => d.Id.Equals(actor.Id, StringComparison.InvariantCultureIgnoreCase));
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

            await m_PermissionFileManager.SaveUsersAsync();
        }

        protected virtual async Task<UserData> GetOrCreateUserDataAsync(IPermissionActor actor)
        {
            var user = m_PermissionFileManager.UsersData.Users.FirstOrDefault(d => d.Id == actor.Id);
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

            m_PermissionFileManager.UsersData.Users.Add(userData);
            await m_PermissionFileManager.SaveUsersAsync();
            return userData;
        }

        public virtual Task<IReadOnlyCollection<IPermissionGroup>> GetGroupsAsync()
        {
            return Task.FromResult((IReadOnlyCollection<IPermissionGroup>)m_PermissionFileManager.PermissionGroupsData.PermissionGroups.OfType<IPermissionGroup>().ToList());
        }

        public virtual Task<IPermissionGroup> GetGroupAsync(string id)
        {
            return Task.FromResult(m_PermissionFileManager.PermissionGroupsData.PermissionGroups.OfType<IPermissionGroup>().FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual async Task<bool> UpdateGroupAsync(IPermissionGroup group)
        {
            if (await GetGroupAsync(group.Id) == null)
            {
                return false;
            }

            m_PermissionFileManager.PermissionGroupsData.PermissionGroups.RemoveAll(d => d.Id.Equals(group.Id, StringComparison.OrdinalIgnoreCase));
            m_PermissionFileManager.PermissionGroupsData.PermissionGroups.Add((PermissionGroup)group);

            await m_PermissionFileManager.SavePermissionGroupsAsync();
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
            await m_PermissionFileManager.SaveUsersAsync();
            return true;
        }

        public virtual async Task<bool> RemoveGroupFromActorAsync(IPermissionActor actor, string groupId)
        {
            var user = await GetOrCreateUserDataAsync(actor);
            user.Groups.Remove(groupId);

            await m_PermissionFileManager.SaveUsersAsync();
            return true;
        }

        public virtual async Task<bool> CreateGroupAsync(IPermissionGroup group)
        {
            if (await GetGroupAsync(group.Id) != null)
            {
                return false;
            }

            m_PermissionFileManager.PermissionGroupsData.PermissionGroups.Add(new PermissionGroupData
            {
                Priority = group.Priority,
                Id = group.Id,
                DisplayName = group.DisplayName,
                Data = group.Data ?? new Dictionary<string, object>(),
                Parents = new HashSet<string>(group.Parents, StringComparer.InvariantCultureIgnoreCase),
                Permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase),
                IsAutoAssigned = group.IsAutoAssigned
            });

            await m_PermissionFileManager.SavePermissionGroupsAsync();
            return true;
        }

        public virtual async Task<bool> DeleteGroupAsync(string groupId)
        {
            if (m_PermissionFileManager.PermissionGroupsData.PermissionGroups.RemoveAll(c => groupId.Equals(c.Id, StringComparison.OrdinalIgnoreCase)) == 0)
            {
                return false;
            }

            await m_PermissionFileManager.SavePermissionGroupsAsync();
            return true;
        }

        protected IEnumerable<IPermissionGroup> GetAutoAssignPermissionGroups()
        {
            return m_PermissionFileManager.PermissionGroupsData.PermissionGroups
                .OfType<IPermissionGroup>()
                .Where(d => d.IsAutoAssigned);
        }
    }
}