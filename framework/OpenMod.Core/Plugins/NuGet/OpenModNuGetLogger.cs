using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using OpenMod.API;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = NuGet.Common.LogLevel;

namespace OpenMod.Core.Plugins.NuGet
{
    [OpenModInternal]
    public class OpenModNuGetLogger : LoggerBase
    {
        private readonly ILogger m_Logger;

        public OpenModNuGetLogger(ILogger logger)
        {
            m_Logger = logger;
        }

        public override void Log(ILogMessage message)
        {
            if (message.Message.Contains("Resolving dependency information took"))
            {
                return;
            }

            if (message.Message.Trim().StartsWith("CACHE"))
            {
                return;
            }

            var level = message.Level;
            var text = message.Message;

            switch (level)
            {
                // ReSharper disable TemplateIsNotCompileTimeConstantProblem
                case LogLevel.Debug:
                    m_Logger.LogDebug(text);
                    break;
                case LogLevel.Verbose:
                    m_Logger.LogTrace(text);
                    break;
                case LogLevel.Information:
                    m_Logger.LogInformation(text);
                    break;
                case LogLevel.Minimal:
                    m_Logger.LogInformation(text);
                    break;
                case LogLevel.Warning:
                    m_Logger.LogWarning(text);
                    break;
                case LogLevel.Error:
                    m_Logger.LogError(text);
                    break;
                // ReSharper restore TemplateIsNotCompileTimeConstantProblem
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}