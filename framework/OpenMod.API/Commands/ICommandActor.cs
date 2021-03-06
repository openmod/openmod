using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents an actor that can execute commands.
    /// </summary>
    public interface ICommandActor : IPermissionActor
    {
        /// <summary>
        /// Prints a message to the actor.
        /// </summary>
        /// <param name="message">The message to print.</param>
        Task PrintMessageAsync(string message);

        /// <summary>
        /// Prints a message with icon to the actor.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="iconUrl">The icon URL to print. May not be supported on all platforms.</param>
        Task PrintMessageAsync(string message, string? iconUrl);

        /// <summary>
        /// Prints a colored message to the actor.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="color">The color of the message.</param>
        Task PrintMessageAsync(string message, Color color);

        /// <summary>
        /// Prints a colored message with icon to the actor.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="iconUrl">The icon URL to print. May not be supported on all platforms.</param>
        Task PrintMessageAsync(string message, Color color, string? iconUrl);
    }
}