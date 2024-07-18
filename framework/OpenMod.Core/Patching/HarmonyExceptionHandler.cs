using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using OpenMod.API;

namespace OpenMod.Core.Patching
{
    [OpenModInternal]
    public static class HarmonyExceptionHandler
    {
        public delegate ILoggerFactory LoggerFactoryGetter();
        public static event LoggerFactoryGetter? LoggerFactoryGetterEvent;

        public static void ReportCleanupException(Type type, Exception? exception, MethodBase? originalMethod)
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
            logger.LogError(exception, "Failed to patch original method {MethodName} from patching type {TypeName}", originalMethod?.FullDescription(), type.FullDescription());
        }
    }
}