using System;
using System.Collections.Generic;

namespace OpenMod.Core.Helpers.Prioritization
{
    public class PriorityComparer : Comparer<API.Helpers.Prioritization.Priority>
    {
        private readonly PriortyComparisonMode m_ComparisonMode;

        public PriorityComparer(PriortyComparisonMode comparisonMode)
        {
            m_ComparisonMode = comparisonMode;
        }

        public override int Compare(API.Helpers.Prioritization.Priority x, API.Helpers.Prioritization.Priority y)
        {
            switch (m_ComparisonMode)
            {
                case PriortyComparisonMode.LowestFirst:
                    return ((int)y).CompareTo((int)x);
                case PriortyComparisonMode.HighestFirst:
                    return ((int)x).CompareTo((int)y);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}