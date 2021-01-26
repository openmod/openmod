using System;
using Microsoft.Extensions.Logging;
using Rocket.Core.Logging;

namespace OpenMod.Unturned.RocketMod.Patches
{
    internal static class RocketModLogPatches
    {
        public delegate void RocketLog(LogLevel level, string message, Exception? ex);

        public static event RocketLog? OnRocketLog;

        public static bool PreLogInternal(ELogType type, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return false;
            }

            LogLevel logLevel;
            switch (type)
            {
                case ELogType.Info:
                    logLevel = LogLevel.Information;
                    break;
                case ELogType.Error:
                    logLevel = LogLevel.Error;
                    break;
                case ELogType.Exception:
                    logLevel = LogLevel.Error;
                    break;
                case ELogType.Warning:
                    logLevel = LogLevel.Warning;
                    break;
                case ELogType.Undefined:
                    logLevel = LogLevel.Information;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            OnRocketLog?.Invoke(logLevel, message, null);
            return false;
        }

        public static bool PreLogException(Exception ex, string message)
        {
            OnRocketLog?.Invoke(LogLevel.Error, message, ex);
            return false;
        }
    }
}