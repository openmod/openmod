using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Extensions.Logging;

namespace OpenMod.Unturned.Patching
{
    internal static class HarmonyExceptionHandler
    {
        internal delegate ILoggerFactory LoggerFactoryGetter();
        internal static event LoggerFactoryGetter? LoggerFactoryGetterEvent;

        internal static void ReportCleanupException(Type type, Exception? exception, MethodBase originalMethod)
        {
            if (exception == null)
            {
                return;
            }

            var loggerFactory = LoggerFactoryGetterEvent?.Invoke();
            if (loggerFactory == null)
            {
                return;
            }

            var logger = loggerFactory.CreateLogger("OpenMod.Harmony");
            logger.LogError(exception, "Failed to patch original method {MethodName} from patching type {TypeName}",
                originalMethod.FullDescription(), type.FullDescription());
        }
    }
}