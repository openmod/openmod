using System;
using System.Collections.Generic;

namespace OpenMod.Core.Helpers
{
    public static class StringHelper
    {
        public static IEnumerable<int> AllIndexesOf(this string str, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("The string to find may not be null or empty", nameof(value));
            }

            for (var index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, stringComparison);
                if (index == -1)
                {
                    yield break;
                }

                yield return index;
            }
        }
    }
}