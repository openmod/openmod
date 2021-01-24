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
    public class SystemCommandJobExecutor : IJobExecutor
    {
        private readonly IRuntime m_Runtime;
        private readonly ILogger<SystemCommandJobExecutor> m_Logger;

        public SystemCommandJobExecutor(
            IRuntime runtime,
            ILogger<SystemCommandJobExecutor> logger)
        {
            m_Runtime = runtime;
            m_Logger = logger;
        }
        public bool SupportsType(string jobType)
        {
            return string.Equals(jobType, "system_command", StringComparison.OrdinalIgnoreCase);
        }

        public async Task ExecuteAsync(ScheduledJob job)
        {
            if (!job.Args.ContainsKey("commands"))
            {
                throw new Exception($"Job \"{job.Name}\" is missing the command list in args.commands!");
            }

            var commands = (IEnumerable<object>)job.Args["commands"];
            foreach (string command in commands)
            {
                m_Logger.LogInformation($"[{job.Name}] Running system command: {command}");
                var args = ArgumentsParser.ParseArguments(command);
                var startInfo = new ProcessStartInfo(args[0], command.Replace(args[0] + " ", string.Empty))
                {
                    UseShellExecute = false,
                    WorkingDirectory = m_Runtime.WorkingDirectory
                };

                var process = Process.Start(startInfo);
                await process.WaitForExitAsync();
            }
        }
    }
}