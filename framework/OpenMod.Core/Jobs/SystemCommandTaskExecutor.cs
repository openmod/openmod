using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Jobs;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Jobs
{
    public class SystemCommandTaskExecutor : CommandTaskExecutor
    {
        private readonly ILogger<SystemCommandTaskExecutor> m_Logger;
        private readonly IOpenModComponent m_OpenModComponent;

        public SystemCommandTaskExecutor(
            IOpenModComponent openModComponent,
            ILogger<SystemCommandTaskExecutor> logger)
        {
            m_OpenModComponent = openModComponent;
            m_Logger = logger;
        }

        public override string TaskType => "system_command";

        protected override Task ExecuteCommandTask(string command, JobTask task)
        {
            m_Logger.LogInformation("[{JobName}] Running system command: {Command}", task.JobName, command);

            var args = ArgumentsParser.ParseArguments(command);
            var startInfo = new ProcessStartInfo(args[0], command.Replace(args[0] + " ", string.Empty))
            {
                UseShellExecute = false,
                WorkingDirectory = m_OpenModComponent.WorkingDirectory
            };

            var process = Process.Start(startInfo);
            return process!.WaitForExitAsync();
        }
    }
}