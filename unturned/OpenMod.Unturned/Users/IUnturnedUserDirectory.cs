extern alias JetBrainsAnnotations;
using System.Collections.Generic;
using JetBrainsAnnotations::JetBrains.Annotations;
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
        /// <returns>The Unturned user instance of the given player. Cannot return null.</returns>
        [NotNull]
        UnturnedUser GetUser(Player player);

        /// <summary>
        /// Gets the Unturned user instance for the given pending player.
        /// </summary>
        /// <param name="pending">The pending player to get the Unturned user of.</param>
        /// <returns>The Unturned user instance of the given pending player. Cannot return null.</returns>
        [NotNull]
        UnturnedPendingUser GetPendingUser(SteamPending pending);

        /// <summary>
        /// Tries to find a Unturned user based on Steam ID.
        /// </summary>
        /// <param name="steamId">The Steam ID of the Unturned user to look for.</param>
        /// <returns><b>The Unturned user</b> if found; otherwise, <b>null</b>.</returns>
        [CanBeNull]
        UnturnedUser FindUser(CSteamID steamId);

        /// <summary>
        /// Tries to find a pending Unturned player user based on Steam ID.
        /// </summary>
        /// <param name="steamId">The Steam ID of the pending Unturned user to look for.</param>
        /// <returns><b>The pending Unturned user</b> if found; otherwise, <b>null</b>.</returns>
        [CanBeNull]
        UnturnedPendingUser FindPendingUser(CSteamID steamId);

        /// <summary>
        /// Tries to find a pending Unturned player user based on ID or name depending on <see cref="searchMode"/>.
        /// </summary>
        /// <param name="searchString">Name or ID of the user. The value depends on <see cref="searchMode"/>.</param>
        /// <param name="searchMode">The search mode. See <see cref="UserSearchMode"/>.</param>
        /// <returns><b>The Unturned user</b> if found; otherwise, <b>null</b>.</returns>
        [CanBeNull]
        UnturnedUser FindUser(string searchString, UserSearchMode searchMode);

        /// <summary>
        /// Gets all online Unturned users.
        /// </summary>
        /// <returns>All online Unturned users. Cannot return null and neither can the items be null.</returns>
        [NotNull]
        [ItemNotNull]
        ICollection<UnturnedUser> GetOnlineUsers();

        /// <summary>
        /// Gets all pending Unturned users.
        /// </summary>
        /// <returns>All pending Unturned users. Cannot return null and neither can the items be null.</returns>
        [NotNull]
        [ItemNotNull]
        ICollection<UnturnedPendingUser> GetPendingUsers();
    }
}