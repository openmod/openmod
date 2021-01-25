using System.Threading.Tasks;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        Task ExecuteAsync();
    }
}