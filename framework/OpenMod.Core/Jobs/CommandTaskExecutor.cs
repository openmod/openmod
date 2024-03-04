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
            if (!task.Args.ContainsKey("commands"))
                throw new Exception($"Job \"{task.JobName}\" is missing the command list in args.commands!");

            var commands = (IEnumerable<object?>)task.Args["commands"]!;
            foreach (var command in commands.Cast<string?>())
            {
                if (string.IsNullOrEmpty(command)) continue;

                await ExecuteCommandTask(command!, task);
            }
        }

        protected abstract Task ExecuteCommandTask(string command, JobTask task);
    }
}