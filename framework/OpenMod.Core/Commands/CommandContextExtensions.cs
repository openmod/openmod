using System.Text;
using OpenMod.API.Commands;
using SmartFormat.ZString;

namespace OpenMod.Core.Commands
{
    public static class CommandContextExtensions
    {
        public static string GetCommandLine(this ICommandContext context, bool includeArguments = true)
        {
            using var sb = new ZStringBuilder(false);
            sb.Append(context.CommandPrefix);
            sb.Append(context.CommandAlias);

            if (includeArguments)
            {
                foreach (var argument in context.Parameters)
                {
                    sb.Append(" ");

                    // escape " and '
                    var escapedArgument = argument
                        .Replace("\"", "\\\"")
                        .Replace("'", "\\'");

                    // argument contains spaces hence it needs to be on quotes
                    var useQuotes = escapedArgument.Contains(" ");

                    if (useQuotes)
                    {
                        sb.Append("\"");
                    }

                    sb.Append(escapedArgument);

                    if (useQuotes)
                    {
                        sb.Append("\"");
                    }
                }
            }

            return sb.ToString();
        }
    }
}