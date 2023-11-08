using System;
using System.Collections;
using System.Collections.Generic;
using OpenMod.API.Prioritization;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Prioritization
{
    public class PriorityComparer : Comparer<Priority>, IComparer<ServiceRegistration>
    {
        private readonly PriortyComparisonMode m_ComparisonMode;

        public PriorityComparer(PriortyComparisonMode comparisonMode)
        {
            m_ComparisonMode = comparisonMode;
        }

        public override int Compare(Priority x, Priority y)
        {
            return m_ComparisonMode switch
            {
                PriortyComparisonMode.LowestFirst => ((int)x).CompareTo((int)y),
                PriortyComparisonMode.HighestFirst => ((int)y).CompareTo((int)x),
                _ => throw new ArgumentException(nameof(m_ComparisonMode))
            };
        }

        public int Compare(ServiceRegistration x, ServiceRegistration y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}