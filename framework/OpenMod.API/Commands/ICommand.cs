using System.Threading.Tasks;

namespace OpenMod.API.Commands
{
    public interface ICommand
    {
        /// <summary>
        ///     Executes the command if no child command is found.
        /// </summary>
        Task ExecuteAsync();
    }
}