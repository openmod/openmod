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
            var args = new List<string>();

            var currentArg = new StringBuilder(32);

            var inQuote = false;
            var inApostrophes = false;
            var isEscaped = false;

            for (var i = 0; i < line.Length; i++)
            {
                var currentChar = line[i];
                if (isEscaped)
                {
                    currentArg.Append(currentChar);
                    isEscaped = false;
                    continue;
                }

                var nextIndex = i + 1;
                switch (currentChar)
                {
                    case '\\':
                        isEscaped = true;
                        break;

                    case '\'':
                        if (!inQuote && (currentArg.Length == 0 || line.Length == nextIndex || IsSpace(line[nextIndex])))
                        {
                            inApostrophes = !inApostrophes;
                        }
                        else
                        {
                            currentArg.Append(currentChar);
                        }
                        break;

                    case '"':
                        if (!inApostrophes && (currentArg.Length == 0 || line.Length == nextIndex || IsSpace(line[nextIndex])))
                        {
                            inQuote = !inQuote;
                        }
                        else
                        {
                            currentArg.Append(currentChar);
                        }
                        break;

                    default:
                        if (IsSpace(currentChar))
                        {
                            if (inQuote || inApostrophes)
                            {
                                currentArg.Append(currentChar);
                            }
                            else if (currentArg.Length != 0)
                            {
                                args.Add(currentArg.ToString());
                                currentArg.Clear();
                            }
                            break;
                        }

                        currentArg.Append(currentChar);
                        break;
                }
            }

            if (inQuote || inApostrophes) //command: 'command "player    ' -> args: 'command', 'player'
                currentArg = TrimEnd(currentArg);

            if (currentArg.Length != 0)
                args.Add(currentArg.ToString());

            return args.ToArray();
        }

        private static StringBuilder TrimEnd(StringBuilder currentArg)
        {
            var lenght = 0;
            for (var i = currentArg.Length - 1; 0 <= i; i--)
            {
                if (!IsSpace(currentArg[i]))
                {
                    if (lenght != 0)
                        currentArg.Remove(i + 1, lenght);

                    break;
                }

                lenght++;
            }

            return currentArg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSpace(char character)
        {
            return char.IsWhiteSpace(character) || character == char.MinValue;
        }
    }
}