using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Jobs;

namespace OpenMod.Core.Jobs
{
    public abstract class CommandTaskExecutor : ITaskExecutor
    {
        public abstract string TaskType { get; }

        public virtual bool SupportsType(string taskType)
        {
            return string.Equals(taskType, TaskType, StringComparison.OrdinalIgnoreCase);
        }

        public async Task ExecuteAsync(JobTask task)
        {
            if (!task.Args.TryGetValue("commands", out var commands) || commands == null)
            {
                throw new Exception($"Job \"{task.JobName}\" is missing the command list in args.commands!");
            }

            var cmds = ((IEnumerable<object?>)commands).Cast<string?>().Where(c => !string.IsNullOrEmpty(c));
            foreach (var command in cmds)
            {
                await ExecuteCommandTask(command!, task);
            }
        }

        protected abstract Task ExecuteCommandTask(string command, JobTask task);
    }
}