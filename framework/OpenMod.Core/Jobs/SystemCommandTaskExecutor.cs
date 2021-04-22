using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Jobs;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Jobs
{
    public class SystemCommandTaskExecutor : ITaskExecutor
    {
        private readonly IRuntime m_Runtime;
        private readonly ILogger<SystemCommandTaskExecutor> m_Logger;

        public SystemCommandTaskExecutor(
            IRuntime runtime,
            ILogger<SystemCommandTaskExecutor> logger)
        {
            m_Runtime = runtime;
            m_Logger = logger;
        }
        public bool SupportsType(string taskType)
        {
            return string.Equals(taskType, "system_command", StringComparison.OrdinalIgnoreCase);
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

                m_Logger.LogInformation("[{JobName}] Running system command: {Command}", task.JobName, command!);
                var args = ArgumentsParser.ParseArguments(command!);
                var startInfo = new ProcessStartInfo(args[0], command!.Replace(args[0] + " ", string.Empty))
                {
                    UseShellExecute = false,
                    WorkingDirectory = m_Runtime.WorkingDirectory
                };

                var process = Process.Start(startInfo);
                await process!.WaitForExitAsync();
            }
        }
    }
}