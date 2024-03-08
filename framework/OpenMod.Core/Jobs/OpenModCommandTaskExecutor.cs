using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API.Commands;
using OpenMod.API.Jobs;
using OpenMod.Core.Helpers;
using OpenMod.Core.Localization;

namespace OpenMod.Core.Jobs
{
    public class OpenModCommandTaskExecutor : CommandTaskExecutor
    {
        private readonly OpenModCommandTaskActor m_Actor;
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly ILogger<OpenModCommandTaskExecutor> m_Logger;
        private readonly IOptions<SmartFormatOptions> m_SmartFormatOptions;

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

        public override string TaskType => "openmod_command";

        protected override Task ExecuteCommandTask(string command, JobTask task)
        {
            m_Logger.LogInformation("[{JobName}] Running OpenMod command: {Command}", task.JobName, command);

            var formatter = m_SmartFormatOptions.Value.GetSmartFormatter();
            var formattedCommand = formatter.Format(command, task.Parameters);

            var args = ArgumentsParser.ParseArguments(formattedCommand);
            return m_CommandExecutor.ExecuteAsync(m_Actor, args, string.Empty);
        }
    }
}