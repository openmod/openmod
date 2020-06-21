using OpenMod.API.Prioritization;
using OpenMod.Core.Permissions;
using OpenMod.Unturned.Users;

namespace OpenMod.Unturned.Permissions
{
    [Priority(Priority = Priority.High)]
    public class UnturnedAdminPermissionCheckProvider: AlwaysGrantPermissionCheckProvider
    {
        public UnturnedAdminPermissionCheckProvider() : base(actor => actor is UnturnedUser user && user.SteamPlayer.isAdmin)
        {
        }
    }
}
