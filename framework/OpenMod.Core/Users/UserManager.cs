using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API;

namespace OpenMod.Core.Users
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UserManager : IUserManager, IAsyncDisposable
    {
        private bool m_IsDisposing;

        private readonly List<IUserProvider> m_UserProviders;

        public UserManager(IOptions<UserManagerOptions> options, IServiceProvider serviceProvider)
        {
            m_UserProviders = new List<IUserProvider>();
            foreach (var provider in options.Value.UserProviderTypes)
            {
                System.Console.WriteLine("Activating: " + provider.FullName);
                m_UserProviders.Add((IUserProvider)ActivatorUtilities.CreateInstance(serviceProvider, provider));
            }
        }

        public IReadOnlyCollection<IUserProvider> UserProviders
        {
            get { return m_UserProviders; }
        }

        public virtual async Task<IReadOnlyCollection<IUser>> GetUsersAsync(string type)
        {
            var list = new List<IUser>();

            foreach (var userProvider in UserProviders.Where(d => d.SupportsUserType(type)))
            {
                list.AddRange(await userProvider.GetUsersAsync(type));
            }

            return list;
        }

        public virtual async Task<IUser> FindUserAsync(string type, string searchString, UserSearchMode searchMode)
        {
            foreach (var userProvider in UserProviders.Where(d => d.SupportsUserType(type)))
            {
                var user = await userProvider.FindUserAsync(type, searchString, searchMode);
                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }

        public virtual async Task BroadcastAsync(string message)
        {
            foreach (var provider in UserProviders)
            {
                await provider.BroadcastAsync(message);
            }
        }

        public virtual async Task BroadcastAsync(string userType, string message)
        {
            var provider = UserProviders.FirstOrDefault(d => d.SupportsUserType(userType));
            if (provider == null)
            {
                return;
            }

            await provider.BroadcastAsync(userType, message);
        }

        public virtual async Task BroadcastAsync(string message, Color? color)
        {
            foreach (var provider in UserProviders)
            {
                await provider.BroadcastAsync(message, color);
            }
        }

        public virtual async Task BroadcastAsync(string userType, string message, Color? color)
        {
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