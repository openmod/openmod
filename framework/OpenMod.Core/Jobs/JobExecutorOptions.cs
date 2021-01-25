using System;
using System.Collections.Generic;
using OpenMod.API.Jobs;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Jobs
{
    public class JobExecutorOptions
    {
        private readonly List<Type> m_JobExecutorTypes;
        private readonly PriorityComparer m_PriorityComparer;
        public IReadOnlyCollection<Type> JobExecutorTypes => m_JobExecutorTypes;

        public JobExecutorOptions()
        {
            m_PriorityComparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
            m_JobExecutorTypes = new List<Type>();
        }

        public void AddJobExecutor<TProvider>() where TProvider : ITaskExecutor
        {
            AddJobExecutor(typeof(TProvider));
        }

        public void AddJobExecutor(Type type)
        {
            if (!typeof(ITaskExecutor).IsAssignableFrom(type))
            {
                throw new Exception($"Type {type} must be an instance of IJobExecutor!");
            }

            if (m_JobExecutorTypes.Contains(type))
            {
                return;
            }

            m_JobExecutorTypes.Add(type);
            m_JobExecutorTypes.Sort((a, b) => m_PriorityComparer.Compare(a.GetPriority(), b.GetPriority()));
        }

        public bool RemoveJobExecutor(Type type)
        {
            return m_JobExecutorTypes.RemoveAll(d => d == type) > 0;
        }

        public void RemoveJobExecutor<TProvider>() where TProvider : ITaskExecutor
        {
            RemoveJobExecutor(typeof(TProvider));
        }
    }
}