using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;
using OpenMod.API.Localization;

namespace OpenMod.Core.Commands
{
    public class NotEnoughPermissionException : UserFriendlyException
    {
        public NotEnoughPermissionException(string permission, IStringLocalizer stringLocalizer) : base(stringLocalizer["commands:errors:not_enough_permission", new { Permission = permission }])
        {
        }

        public NotEnoughPermissionException(ICommandContext context, string permission) : this(permission, context.ServiceProvider.GetRequiredService<IOpenModStringLocalizer>())
        {

        }
    }
}