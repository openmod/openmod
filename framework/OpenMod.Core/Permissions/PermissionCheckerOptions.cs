using System;
using System.Collections.Generic;
using OpenMod.API.Permissions;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Permissions
{
    public class PermissionCheckerOptions
    {
        private readonly List<Type> m_PermissionCheckProviderTypes;
        private readonly List<Type> m_PermissionSourceTypes;
        private readonly PriorityComparer m_PriorityComparer;

        public PermissionCheckerOptions()
        {
            m_PermissionCheckProviderTypes = new List<Type>();
            m_PermissionSourceTypes = new List<Type>();
            m_PriorityComparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
        }

        public IReadOnlyCollection<Type> PermissionCheckProviders => m_PermissionCheckProviderTypes;

        public IReadOnlyCollection<Type> PermissionSources => m_PermissionSourceTypes;

        public void AddPermissionCheckProvider<TProvider>() where TProvider: IPermissionCheckProvider
        {
            AddPermissionCheckProvider(typeof(TProvider));
        }

        public void AddPermissionCheckProvider(Type type)
        {
            if (!typeof(IPermissionCheckProvider).IsAssignableFrom(type))
            {
                throw new Exception($"Type {type} must be an instance of IPermissionCheckProvider!");
            }

            if (m_PermissionCheckProviderTypes.Contains(type))
            {
                return;
            }

            m_PermissionCheckProviderTypes.Add(type);
            m_PermissionCheckProviderTypes.Sort((a, b) => m_PriorityComparer.Compare(a.GetPriority(), b.GetPriority()));
        }

        public void RemovePermissionCheckProvider<TProvider>() where TProvider : IPermissionCheckProvider
        {
            RemovePermissionCheckProvider(typeof(TProvider));
        }

        public bool RemovePermissionCheckProvider(Type type)
        {
            return m_PermissionCheckProviderTypes.RemoveAll(d => d == type) > 0;
        }

        public void AddPermissionSource<TSource>() where TSource : IPermissionStore
        {
            AddPermissionSource(typeof(TSource));
        }

        public void AddPermissionSource(Type type)
        {
            if (!typeof(IPermissionStore).IsAssignableFrom(type))
            {
                throw new Exception($"Type {type} must be an instance of IPermissionSource!");
            }

            if (m_PermissionSourceTypes.Contains(type))
            {
                return;
            }

            m_PermissionSourceTypes.Add(type);
            m_PermissionSourceTypes.Sort((a, b) => m_PriorityComparer.Compare(a.GetPriority(), b.GetPriority()));
        }

        public void RemovePermissionSource<TSource>() where TSource : IPermissionStore
        {
            RemovePermissionSource(typeof(TSource));
        }

        public bool RemovePermissionSource(Type type)
        {
            return m_PermissionSourceTypes.RemoveAll(d => d == type) > 0;
        }
    }
}