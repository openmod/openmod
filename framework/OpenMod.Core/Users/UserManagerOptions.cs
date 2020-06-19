using System;
using System.Collections.Generic;
using OpenMod.API.Users;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Users
{
    public class UserManagerOptions
    {
        private readonly List<Type> m_UserProviderTypes;
        private readonly PriorityComparer m_PriorityComparer;
        public IReadOnlyCollection<Type> UserProviderTypes => m_UserProviderTypes;

        public UserManagerOptions()
        {
            m_PriorityComparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
            m_UserProviderTypes = new List<Type>();
        }

        public void AddUserProvider<TProvider>() where TProvider : IUserProvider
        {
            AddUserProvider(typeof(TProvider));
        }

        public void AddUserProvider(Type type)
        {
            if (!typeof(IUserProvider).IsAssignableFrom(type))
            {
                throw new Exception($"Type {type} must be an instance of IUserProvider!");
            }

            if (m_UserProviderTypes.Contains(type))
            {
                return;
            }

            m_UserProviderTypes.Add(type);
            m_UserProviderTypes.Sort((a, b) => m_PriorityComparer.Compare(a.GetPriority(), b.GetPriority()));
        }

        public bool RemoveUserProvider(Type type)
        {
            return m_UserProviderTypes.RemoveAll(d => d == type) > 0;
        }

        public void RemoveUserProvider<TProvider>() where TProvider : IUserProvider
        {
            RemoveUserProvider(typeof(TProvider));
        }
    }
}