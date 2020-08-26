using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;
using OpenMod.API.Localization;

namespace OpenMod.Core.Commands
{
    public class NotEnoughPermissionException : UserFriendlyException
    {
        public NotEnoughPermissionException(IStringLocalizer stringLocalizer, string permission) : base(stringLocalizer["commands:errors:not_enough_permission", new { Permission = permission }])
        {
        }

        public NotEnoughPermissionException(ICommandContext context, string permission) : this(context.ServiceProvider.GetRequiredService<IOpenModStringLocalizer>(), GetPermission(context, permission))
        {

        }

        private static string GetPermission(ICommandContext context, string permission)
        {
            var permissionBuilder = context.ServiceProvider.GetRequiredService<ICommandPermissionBuilder>();
            return $"{permissionBuilder.GetPermission(context.CommandRegistration)}.{permission}";
        }
    }
}