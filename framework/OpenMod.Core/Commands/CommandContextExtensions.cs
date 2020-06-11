using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public static class CommandContextExtensions
    {
        public static string GetCommandLine(this ICommandContext context)
        {
            return context.CommandPrefix + context.CommandAlias + " " + string.Join(" ", context.Parameters);
        }
    }
}