using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Users
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UserManager : IUserManager, IAsyncDisposable
    {
        private bool m_IsDisposing;

        private readonly List<IUserProvider> m_UserProviders;

        public UserManager(IOptions<UserManagerOptions> options, ILifetimeScope lifetimeScope)
        {
            m_UserProviders = new List<IUserProvider>();

            foreach (var provider in options.Value.UserProviderTypes)
            {
                m_UserProviders.Add((IUserProvider)ActivatorUtilitiesEx.CreateInstance(lifetimeScope, provider));
            }
        }

        public IReadOnlyCollection<IUserProvider> UserProviders
        {
            get { return m_UserProviders; }
        }

        public virtual async Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType)
        {
            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            var list = new List<IUser>();

            foreach (var userProvider in UserProviders.Where(d => d.SupportsUserType(userType)))
            {
                list.AddRange(await userProvider.GetUsersAsync(userType));
            }

            return list;
        }

        public virtual async Task<IUser?> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            if (string.IsNullOrEmpty(searchString))
            {
                throw new ArgumentException(nameof(searchString));
            }

            foreach (var userProvider in UserProviders.Where(d => d.SupportsUserType(userType)))
            {
                var user = await userProvider.FindUserAsync(userType, searchString, searchMode);
                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }

        public virtual async Task BroadcastAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            foreach (var provider in UserProviders)
            {
                await provider.BroadcastAsync(message);
            }
        }

        public virtual async Task BroadcastAsync(string userType, string message)
        {
            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            var provider = UserProviders.FirstOrDefault(d => d.SupportsUserType(userType));
            if (provider == null)
            {
                return;
            }

            await provider.BroadcastAsync(userType, message);
        }

        public virtual Task BroadcastAsync(string message, Color? color)
        {
            return BroadcastWithIconAsync(message, color: color);
        }

        public virtual async Task BroadcastWithIconAsync(string message, string? iconUrl = null, Color? color = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException($"'{nameof(message)}' cannot be null or empty", nameof(message));
            }

            foreach (var provider in UserProviders)
            {
                await provider.BroadcastWithIconAsync(message, iconUrl, color);
            }
        }

        public virtual Task BroadcastAsync(string userType, string message, Color? color)
        {
            return BroadcastAsync(userType, message, color: color);
        }

        public virtual async Task BroadcastWithIconAsync(string userType, string message, string? iconUrl = null, Color? color = null)
        {
            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException($"'{nameof(userType)}' cannot be null or empty", nameof(userType));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException($"'{nameof(message)}' cannot be null or empty", nameof(message));
            }

            var provider = UserProviders.FirstOrDefault(d => d.SupportsUserType(userType));
            if (provider == null)
            {
                return;
            }

            await provider.BroadcastAsync(userType, message, color);
        }

        public virtual ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return new ValueTask(Task.CompletedTask);
            }
            m_IsDisposing = true;

            return new ValueTask(m_UserProviders.DisposeAllAsync());
        }
    }
}