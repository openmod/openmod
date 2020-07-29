using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

                if (currentChar.Equals('\\'))
                {
                    isEscaped = true;
                    continue;
                }

                switch (argsBuilder[i])
                {
                    case '"':
                        inQuote = !inQuote;
                        break;

                    case ' ':
                        if (inQuote)
                        {
                            currentArg.Append(argsBuilder[i]);
                            break;
                        }

                        args.Add(currentArg.ToString());
                        currentArg.Clear();
                        break;

                    default:
                        if (char.IsWhiteSpace(argsBuilder[i])) //Space was checked before
                        {
                            break;
                        }

                        currentArg.Append(argsBuilder[i]);
                        break;
                }
            }

            args.Add(currentArg.ToString());
            return args.ToArray();
        }
    }
}