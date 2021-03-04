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

        // https://stackoverflow.com/questions/9453731/how-to-calculate-distance-similarity-measure-of-given-2-strings
        public static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(a))
            {
                return b.Length;
            }
            if (string.IsNullOrEmpty(b))
            {
                return a.Length;
            }

            var lengthA = a.Length;
            var lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];

            for (var i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (var j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (var i = 1; i <= lengthA; i++)
            for (var j = 1; j <= lengthB; j++)
            {
                var cost = b[j - 1] == a[i - 1] ? 0 : 1;
                distances[i, j] = Math.Min
                (
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost
                );
            }
            return distances[lengthA, lengthB];
        }
    }
}