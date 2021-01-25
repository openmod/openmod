using System.Threading.Tasks;

namespace OpenMod.API.Jobs
{
    /// <summary>
    /// Executes a task.
    /// </summary>
    public interface ITaskExecutor
    {
        /// <summary>
        /// Checks if the task executor can execute the given task type.
        /// </summary>
        /// <param name="taskType">The task type to check</param>
        /// <returns></returns>
        bool SupportsType(string taskType);

        /// <summary>
        /// Executes the given task.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        Task ExecuteAsync(JobTask task);
    }
}