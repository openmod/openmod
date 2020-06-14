using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public class CommandWrongUsageException : UserFriendlyException
    {
        public CommandWrongUsageException(string message) : base(message)
        {
            
        }

        public CommandWrongUsageException(ICommandContext context, IStringLocalizer localizer) : base(localizer["commands:errors:wrong_usage", new { CommandLine = context.GetCommandLine(), CommandPrefix = context.CommandPrefix + context.CommandAlias, context.CommandRegistration.Syntax }])
        {

        }

        public CommandWrongUsageException(ICommandContext context) : this(context, context.ServiceProvider.GetRequiredService<IStringLocalizer>())
        {

        }
    }
}