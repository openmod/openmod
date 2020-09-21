using System.Collections.Generic;
using OpenMod.API.Ioc;
using OpenMod.API.Users;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    [Service]
    public interface IUnturnedUserDirectory
    {
        UnturnedUser GetUser(Player player);
        UnturnedPendingUser GetPendingUser(SteamPending pending);
        UnturnedUser FindUser(CSteamID steamId);
        UnturnedPendingUser FindPendingUser(CSteamID steamId);
        UnturnedUser FindUser(string searchString, UserSearchMode searchMode);
        ICollection<UnturnedUser> GetOnlineUsers();
        ICollection<UnturnedPendingUser> GetPendingUsers();
    }
}