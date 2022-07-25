using System.Collections.Generic;
using OpenMod.API.Ioc;
using OpenMod.API.Users;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    /// <summary>
    /// The service used for querying Unturned users. Unlike <see cref="IUserManager"/> these methods are not async and are guaranteed to not block.
    /// </summary>
    [Service]
    public interface IUnturnedUserDirectory
    {
        /// <summary>
        /// Gets the Unturned user instance for the given player.
        /// </summary>
        /// <param name="player">The player to get the Unturned user of.</param>
        /// <returns>The Unturned user instance of the given player.</returns>
        UnturnedUser GetUser(Player player);

        /// <summary>
        /// Gets the Unturned user instance for the given pending player.
        /// </summary>
        /// <param name="pending">The pending player to get the Unturned user of.</param>
        /// <returns>The Unturned user instance of the given pending player.</returns>
        UnturnedPendingUser GetPendingUser(SteamPending pending);

        /// <summary>
        /// Tries to find a Unturned user based on Steam ID.
        /// </summary>
        /// <param name="steamId">The Steam ID of the Unturned user to look for.</param>
        /// <returns><b>The Unturned user</b> if found; otherwise, <b>null</b>.</returns>
        UnturnedUser? FindUser(CSteamID steamId);

        /// <summary>
        /// Tries to find a pending Unturned player user based on Steam ID.
        /// </summary>
        /// <param name="steamId">The Steam ID of the pending Unturned user to look for.</param>
        /// <returns><b>The pending Unturned user</b> if found; otherwise, <b>null</b>.</returns>
        UnturnedPendingUser? FindPendingUser(CSteamID steamId);

        /// <summary>
        /// Tries to find a Unturned player user based on ID or name depending on <see cref="UserSearchMode"/>.
        /// </summary>
        /// <param name="searchString">Name or ID of the user. The value depends on <see cref="UserSearchMode"/>.</param>
        /// <param name="searchMode">The search mode. See <see cref="UserSearchMode"/>.</param>
        /// <returns><b>The Unturned user</b> if found; otherwise, <b>null</b>.</returns>
        UnturnedUser? FindUser(string searchString, UserSearchMode searchMode);

        /// <summary>
        /// Gets all online Unturned users.
        /// </summary>
        /// <returns>All online Unturned users.</returns>
        ICollection<UnturnedUser> GetOnlineUsers();

        /// <summary>
        /// Gets all pending Unturned users.
        /// </summary>
        /// <returns>All pending Unturned users.</returns>
        ICollection<UnturnedPendingUser> GetPendingUsers();
    }
}