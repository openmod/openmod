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
        /// Prints a colored message to the actor.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="color">The color of the message.</param>
        Task PrintMessageAsync(string message, Color color);
    }
}