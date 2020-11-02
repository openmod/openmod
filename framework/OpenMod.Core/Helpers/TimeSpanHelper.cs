using OpenMod.API.Commands;
using System;
using System.Text.RegularExpressions;

namespace OpenMod.Core.Helpers
{
    public static class TimeSpanHelper
    {
        /**
         * Regex Breakdown:
         * (\d+\.?\d*|\d*\.?\d+)\ *[a-zA-Z]+
         *  \d+\.?\d*                           Must start with one or more digits, then match can include a period, then match can end with zero or more digits
         *           |                          or
         *            \d*\.?\d+                 Starts with zero or more digits, then match can include a period, then match must end with one or more digits
         *                      \ *             Can contain spaces
         *                         [a-zA-Z]+    One or more letters
         */
        private static readonly Regex s_RegexPattern = new Regex(@"(\d+\.?\d*|\d*\.?\d+)\ *[a-zA-Z]+");

        /**
         * Examples of unparsed inputs:
         * - 20 seconds
         * - 30 days, 40 minutes, and 50 seconds
         * - 10h20m30s
         * - 3.5 days
         * - 1234.123     milliseconds
         */
        public static TimeSpan Parse(string unparsed)
        {
            var matches = s_RegexPattern.Matches(unparsed);

            TimeSpan total = new TimeSpan();

            if (matches.Count == 0)
            {
                throw new UserFriendlyException("Invalid time span format");
            }

            foreach (Match match in matches)
            {
                string value = match.Value;

                int i;

                for (i = 0; i < value.Length; i++)
                {
                    if ((value[i] < '0' || value[i] > '9') && value[i] != '.') break;
                }

                double num = double.Parse(value.Substring(0, i));
                string suffix = value.Substring(i).Trim();

                switch (suffix.ToLower())
                {
                    case "d":
                    case "day":
                    case "days":
                        total += TimeSpan.FromDays(num);
                        break;
                    case "h":
                    case "hr":
                    case "hrs":
                    case "hour":
                    case "hours":
                        total += TimeSpan.FromHours(num);
                        break;
                    case "m":
                    case "min":
                    case "mins":
                    case "minute":
                    case "minutes":
                        total += TimeSpan.FromMinutes(num);
                        break;
                    case "s":
                    case "sec":
                    case "secs":
                    case "second":
                    case "seconds":
                        total += TimeSpan.FromSeconds(num);
                        break;
                    case "ms":
                    case "milli":
                    case "millis":
                    case "millisecond":
                    case "milliseconds":
                        total += TimeSpan.FromMilliseconds(num);
                        break;
                }
            }

            return total;
        }
    }
}
