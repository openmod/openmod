using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API.Commands;
using OpenMod.API.Jobs;
using OpenMod.Core.Helpers;
using OpenMod.Core.Localization;

namespace OpenMod.Core.Jobs
{
    public class OpenModCommandTaskExecutor : ITaskExecutor
    {
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly ILogger<OpenModCommandTaskExecutor> m_Logger;
        private readonly IOptions<SmartFormatOptions> m_SmartFormatOptions;
        private readonly OpenModCommandTaskActor m_Actor;

        public OpenModCommandTaskExecutor(
            ICommandExecutor commandExecutor,
            ILogger<OpenModCommandTaskExecutor> logger,
            IOptions<SmartFormatOptions> smartFormatOptions)
        {
            m_Actor = new OpenModCommandTaskActor(logger);
            m_CommandExecutor = commandExecutor;
            m_Logger = logger;
            m_SmartFormatOptions = smartFormatOptions;
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

            var commands = (IEnumerable<object?>)task.Args["commands"]!;

            foreach (string? command in commands)
            {
                if (string.IsNullOrEmpty(command))
                {
                    continue;
                }

                m_Logger.LogInformation("[{JobName}] Running OpenMod command: {Command}", task.JobName, command!);

                var formatter = m_SmartFormatOptions.Value.GetSmartFormatter();
                var formattedCommand = formatter.Format(command!, task.Parameters);
                var args = ArgumentsParser.ParseArguments(formattedCommand);

                await m_CommandExecutor.ExecuteAsync(m_Actor, args, string.Empty);
            }
        }
    }
}