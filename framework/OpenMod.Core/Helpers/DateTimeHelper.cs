using OpenMod.API.Commands;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenMod.Core.Helpers
{
    public static class DateTimeHelper
    {
        /**
         * Regex Source: https://regexlib.com/REDetails.aspx?regexp_id=610
         */
        private static readonly Regex s_RegexPattern = new Regex(@"^(?=\d)(?:(?:31(?!.(?:0?[2469]|11))|(?:30|29)(?!.0?2)|29(?=.0?2.(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(?:\x20|$))|(?:2[0-8]|1\d|0?[1-9]))([-./])(?:1[012]|0?[1-9])\1(?:1[6-9]|[2-9]\d)?\d\d(?:(?=\x20\d)\x20|$))?(((0?[1-9]|1[012])(:[0-5]\d){0,2}(\x20[AP]M))|([01]\d|2[0-3])(:[0-5]\d){1,2})?$");

        /**
         * Examples of unparsed inputs:
         * - 21/10/2000
         * - 21.10.2000
         * - 21/10/2000 13:37:31
         * - 21/10/2000 1:37:31 AM
         * - 21/10/2000 21:10
         */
        public static DateTime Parse(string unparsed)
        {
            var matches = s_RegexPattern.Matches(unparsed.Replace(".", "/")); // 21.10.2000 => 21/10/2000

            if (matches.Count != 1)
            {
                throw new UserFriendlyException("Invalid date time format");
            }

            var match = matches[0].Value;

            var dateTimeFormatBuilder = new StringBuilder("d/MM/yyyy");

            if (match.Length > 10)
            {
                var colonCount = match.Count(c => c == ':');

                dateTimeFormatBuilder.Append(colonCount switch
                {
                    0 => " H",
                    1 => " H:mm",
                    2 => " H:mm:ss"
                });

                if (match.EndsWith("M", StringComparison.InvariantCultureIgnoreCase))
                {
                    dateTimeFormatBuilder.Append(" tt");
                }
            }

            return DateTime.ParseExact(match, dateTimeFormatBuilder.ToString(), CultureInfo.InvariantCulture);
        }
    }
}
