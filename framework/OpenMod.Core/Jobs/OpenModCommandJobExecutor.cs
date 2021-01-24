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
    public class OpenModCommandJobExecutor : IJobExecutor
    {
        private readonly IRuntime m_Runtime;
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly ILogger<OpenModCommandJobExecutor> m_Logger;
        private readonly OpenModCommandJobActor m_Actor;

        public OpenModCommandJobExecutor(
            IRuntime runtime,
            ICommandExecutor commandExecutor,
            ILogger<OpenModCommandJobExecutor> logger)
        {
            m_Actor = new OpenModCommandJobActor(logger);
            m_Runtime = runtime;
            m_CommandExecutor = commandExecutor;
            m_Logger = logger;
        }

        public bool SupportsType(string jobType)
        {
            return string.Equals(jobType, "openmod_command", StringComparison.OrdinalIgnoreCase);
        }

        public async Task ExecuteAsync(ScheduledJob job)
        {
            if (!job.Args.ContainsKey("commands"))
            {
                throw new Exception($"Job \"{job.Name}\" is missing the command list in args.commands!");
            }

            var commands = (IEnumerable<object>) job.Args["commands"];
            foreach (string command in commands)
            {
                m_Logger.LogInformation($"[{job.Name}] Running OpenMod command: {command}");

                var args = ArgumentsParser.ParseArguments(command);
                await m_CommandExecutor.ExecuteAsync(m_Actor, args, string.Empty);
            }
        }
    }
}