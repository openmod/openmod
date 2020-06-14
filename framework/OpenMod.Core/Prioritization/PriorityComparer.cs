using System;
using System.Collections.Generic;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Prioritization
{
    public class PriorityComparer : Comparer<Priority>
    {
        private readonly PriortyComparisonMode m_ComparisonMode;

        public PriorityComparer(PriortyComparisonMode comparisonMode)
        {
            m_ComparisonMode = comparisonMode;
        }

        public override int Compare(Priority x, Priority y)
        {
            switch (m_ComparisonMode)
            {
                case PriortyComparisonMode.LowestFirst:
                    return ((int)x).CompareTo((int)y);
                case PriortyComparisonMode.HighestFirst:
                    return ((int)y).CompareTo((int)x);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}