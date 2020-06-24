using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;

namespace OpenMod.Unturned.Permissions
{
    [Priority(Priority = Priority.High)]
    public class UnturnedAdminPermissionCheckProvider : AlwaysGrantPermissionCheckProvider
    {
        public UnturnedAdminPermissionCheckProvider() : base(actor => actor is UnturnedUser user && user.SteamPlayer.isAdmin)
        {
        }
    }
}
