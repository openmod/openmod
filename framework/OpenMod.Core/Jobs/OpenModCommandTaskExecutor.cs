using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Jobs;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Jobs
{
    public class OpenModCommandTaskExecutor : ITaskExecutor
    {
        private readonly IRuntime m_Runtime;
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly ILogger<OpenModCommandTaskExecutor> m_Logger;
        private readonly OpenModCommandTaskActor m_Actor;

        public OpenModCommandTaskExecutor(
            IRuntime runtime,
            ICommandExecutor commandExecutor,
            ILogger<OpenModCommandTaskExecutor> logger)
        {
            m_Actor = new OpenModCommandTaskActor(logger);
            m_Runtime = runtime;
            m_CommandExecutor = commandExecutor;
            m_Logger = logger;
        }

        public bool SupportsType(string taskType)
        {
            return string.Equals(taskType, "openmod_command", StringComparison.OrdinalIgnoreCase);
        }

        public async Task ExecuteAsync(JobTask task)
        {
            if (!task.Args.ContainsKey("commands"))
            {
                throw new Exception($"Job \"{task.JobName}\" is missing the command list in args.commands!");
            }

            var commands = (IEnumerable<object>)task.Args["commands"];
            foreach (string command in commands)
            {
                m_Logger.LogInformation($"[{task.JobName}] Running OpenMod command: {command}");

                var args = ArgumentsParser.ParseArguments(command);
                await m_CommandExecutor.ExecuteAsync(m_Actor, args, string.Empty);
            }
        }
    }
}