using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public static class CommandContextExtensions
    {
        public static string GetCommandLine(this ICommandContext context, bool includeArguments = true)
        {
            return context.CommandPrefix + context.CommandAlias + (includeArguments ? (" " + string.Join(" ", context.Parameters)) : "");
        }
    }
}