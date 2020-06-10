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
        
        public virtual async Task<IReadOnlyCollection<IPermissionGroup>> GetGroupsAsync(IPermissionActor actor)
        {
            var groups = new List<IPermissionGroup>();
            if (actor is IPermissionGroup group)
            {
                foreach (var parentGroupId in group.Parents)
                {
                    var parentGroup = await GetGroupAsync(parentGroupId);
                    if (parentGroup != null)
                    {
                        groups.Add(parentGroup);
                    }
                }

                return groups;
            }

            var user = await GetOrCreateUserDataAsync(actor);
            foreach (var groupId in user.Groups)
            {
                var userGroup = await GetGroupAsync(groupId);
                if (userGroup!= null)
                {
                    groups.Add(userGroup);
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

            var user = m_PermissionFileManager.UsersData.Users.FirstOrDefault(d => d.Id == actor.Id);
            if (user == null)
            {
                // CreateUser will assign groups itself
                await GetOrCreateUserDataAsync(actor);
                return;
            }

            var userGroups = user.Groups.ToList();

            userGroups.AddRange(GetAutoAssignPermissionGroups().Select(d => d.Id));
            user.Groups = userGroups.Distinct().ToList();
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
                Data = new Dictionary<string, object>(),
                Permissions = new List<string>(),
                Groups = GetAutoAssignPermissionGroups().Select(d => d.Id).ToList()
            };

            m_PermissionFileManager.UsersData.Users.Add(userData);
            await m_PermissionFileManager.SaveUsersAsync();
            return userData;
        }

        public virtual Task<IReadOnlyCollection<IPermissionGroup>> GetGroupsAsync()
        {
            // yes, the double casting here is necessary :^)
            return Task.FromResult((IReadOnlyCollection<IPermissionGroup>)m_PermissionFileManager.PermissionGroupsData.PermissionGroups.Select(e => (IPermissionGroup)(PermissionGroup)e).ToList());
        }

        public virtual Task<IPermissionGroup> GetGroupAsync(string id)
        {
            // same double casting professionalism here
            return Task.FromResult((IPermissionGroup)(PermissionGroup)m_PermissionFileManager.PermissionGroupsData.PermissionGroups.FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual async Task<bool> UpdateGroupAsync(IPermissionGroup @group)
        {
            if (await GetGroupAsync(@group.Id) == null)
            {
                return false;
            }

            m_PermissionFileManager.PermissionGroupsData.PermissionGroups.RemoveAll(d => d.Id == @group.Id);
            m_PermissionFileManager.PermissionGroupsData.PermissionGroups.Add((PermissionGroup)@group);

            await m_PermissionFileManager.SavePermissionGroupsAsync();
            return true;
        }

        public virtual async Task<bool> AddGroupAsync(IPermissionActor actor, IPermissionGroup @group)
        {
            var user = await GetOrCreateUserDataAsync(actor);
            if (user.Groups.Any(d => d.Equals(@group.Id, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            user.Groups.Add(@group.Id);
            user.Groups = user.Groups.Distinct().ToList();

            await m_PermissionFileManager.SaveUsersAsync();
            return true;
        }

        public virtual async Task<bool> RemoveGroupAsync(IPermissionActor actor, IPermissionGroup @group)
        {
            var user = await GetOrCreateUserDataAsync(actor);
            user.Groups.RemoveAll(d => d.Equals(@group.Id, StringComparison.OrdinalIgnoreCase));

            user.Groups = user.Groups.Distinct().ToList();
            await m_PermissionFileManager.SaveUsersAsync();
            return true;
        }

        public virtual async Task<bool> CreateGroupAsync(IPermissionGroup @group)
        {
            if (await GetGroupAsync(group.Id) != null)
            {
                return false;
            }

            m_PermissionFileManager.PermissionGroupsData.PermissionGroups.Add(new PermissionGroupData
            {
                Priority = @group.Priority,
                Id = @group.Id,
                DisplayName = @group.DisplayName,
                Data = @group.Data ?? new Dictionary<string, object>(),
                Parents = @group.Parents?.ToList() ?? new List<string>(),
                Permissions = new List<string>(),
                IsAutoAssigned = @group.IsAutoAssigned
            });

            await m_PermissionFileManager.SavePermissionGroupsAsync();
            return true;
        }

        public virtual async Task<bool> DeleteGroupAsync(IPermissionGroup @group)
        {
            if (m_PermissionFileManager.PermissionGroupsData.PermissionGroups.RemoveAll(c => @group.Id.Equals(c.Id, StringComparison.OrdinalIgnoreCase)) == 0)
            {
                return false;
            }

            await m_PermissionFileManager.SavePermissionGroupsAsync();
            return true;
        }

        protected List<PermissionGroup> GetAutoAssignPermissionGroups()
        {
            return m_PermissionFileManager.PermissionGroupsData.PermissionGroups
                .Where(d => d.IsAutoAssigned)
                .Select(d => (PermissionGroup)d)
                .ToList();
        }
    }
}