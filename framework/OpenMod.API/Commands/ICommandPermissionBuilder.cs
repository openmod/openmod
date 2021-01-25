using System.Collections.Generic;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// The service for building command permissions. In other words; this service determines what permission a command requires.
    /// </summary>
    [Service]
    public interface ICommandPermissionBuilder
    {
        /// <summary>
        /// Gets the permission required to use the given command.
        /// </summary>
        /// <param name="command">The command to get the permission for.</param>
        /// <returns>The permission required to use the command.</returns>
        string GetPermission(ICommandRegistration command);

        /// <summary>
        /// Gets the permission required to use the given command.
        /// </summary>
        /// <param name="command">The command to get the permission for.</param>
        /// <param name="commands">The available commands used for determining child command permissions.</param>
        /// <returns>The permission required to use the command.</returns>
        string GetPermission(ICommandRegistration command, IReadOnlyCollection<ICommandRegistration> commands);
    }
}