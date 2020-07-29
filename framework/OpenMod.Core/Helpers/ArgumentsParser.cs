using System.Collections.Generic;
using System.Text;

namespace OpenMod.Core.Helpers
{
    public static class ArgumentsParser
    {
        /// <summary>
        /// C-like argument parser
        /// </summary>
        /// <param name="line">Line string with arguments.</param>
        /// <returns>The args[] array (argv)</returns>
        public static string[] ParseArguments(string line)
        {
            var argsBuilder = new StringBuilder(line);
            var inQuote = false;
            var isEscaped = false;
            var lastIsSpace = true;
            var args = new List<string>();
            var currentArg = new StringBuilder();

            for (var i = 0; i < argsBuilder.Length; i++)
            {
                var currentChar = argsBuilder[i];

                if (isEscaped)
                {
                    currentArg.Append(currentChar);
                    isEscaped = false;
                    continue;
                }

                switch (currentChar)
                {
                    case '\\':
                        isEscaped = true;
                        lastIsSpace = false;
                        break;

                    case '"':
                        inQuote = !inQuote;
                        lastIsSpace = false;
                        break;

                    default:
                        if (char.IsWhiteSpace(currentChar) && !inQuote)
                        {
                            if (lastIsSpace)
                                break;

                            lastIsSpace = true;
                            args.Add(currentArg.ToString());
                            currentArg.Clear();
                            break;
                        }

                        currentArg.Append(argsBuilder[i]);
                        lastIsSpace = false;
                        break;
                }
            }

            args.Add(currentArg.ToString());
            return args.ToArray();
        }
    }
}