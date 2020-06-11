using Microsoft.Extensions.Localization;

namespace OpenMod.Core.Commands
{
    public class NotEnoughPermissionException : UserFriendlyException
    {
        public NotEnoughPermissionException(string permission, IStringLocalizer stringLocalizer) : base(stringLocalizer["commands:errors:not_enough_permission", new { Permission = permission }])
        {
        }
    }
}