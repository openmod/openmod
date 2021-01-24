using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("restart", Priority = Priority.Lowest)]
    [CommandParent(typeof(CommandOpenMod))]
    [CommandDescription("Restarts the server.")]
    public class CommandOpenModRestart : Command
    {
        private readonly IOpenModHost m_Host;
        private readonly ILogger<CommandOpenModRestart> m_Logger;

        public CommandOpenModRestart(
            IServiceProvider serviceProvider,
            IOpenModHost host,
            ILogger<CommandOpenModRestart> logger) : base(serviceProvider)
        {
            m_Host = host;
            m_Logger = logger;
        }

        protected override async Task OnExecuteAsync()
        {
            var args = Environment.GetCommandLineArgs();
            var exe = Process.GetCurrentProcess().MainModule.FileName;
            string arguments;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                arguments = string.Join(" ", args.Select(d =>
                {
                    if (d.Contains(" "))
                    {
                        return "\"" + d.Replace("\"", "^\"") + "\"";
                    }

                    return d.Replace("\"", "^\"");
                }));
            }
            else
            {
                arguments = string.Join(" ", args.Select(d => d.Replace("\"", "\\\"")));
            }

            m_Logger.LogInformation($"Executing command after shutdown: {Environment.NewLine}{exe} {arguments}");

            var startInfo = new ProcessStartInfo(exe, arguments)
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(exe)
            };

            foreach (DictionaryEntry kv in Environment.GetEnvironmentVariables())
            {
                if (startInfo.EnvironmentVariables.ContainsKey(kv.Key.ToString()))
                {
                    startInfo.EnvironmentVariables[kv.Key.ToString()] = kv.Value.ToString();
                }
                else
                {
                    startInfo.EnvironmentVariables.Add(kv.Key.ToString(), kv.Value.ToString());
                }
            }

            await m_Host.ShutdownAsync();
            Process.Start(startInfo);
        }
    }
}