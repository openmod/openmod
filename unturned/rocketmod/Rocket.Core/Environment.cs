using System;
using System.IO;

namespace Rocket.Core
{
    public static class Environment
    {
        public static void Initialize()
        {
            if (!Directory.Exists(PluginsDirectory)) Directory.CreateDirectory(PluginsDirectory);
            if (!Directory.Exists(LibrariesDirectory)) Directory.CreateDirectory(LibrariesDirectory);
            if (!Directory.Exists(LogsDirectory)) Directory.CreateDirectory(LogsDirectory);
            string logFile = Path.Combine(LogsDirectory,LogFile);
            if (File.Exists(logFile))
            {
                string ver = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                File.Move(logFile, Path.Combine(LogsDirectory, String.Format(LogBackupFile, ver)));
            };
        }

        public static readonly string PluginsDirectory = "Plugins";
        public static readonly string LibrariesDirectory = "Libraries";
        public static readonly string LogsDirectory = "Logs";

        public static readonly string SettingsFile = "Rocket.config.xml";
        public static readonly string TranslationFile = "Rocket.{0}.translation.xml";
        public static readonly string LogFile = "Rocket.log";
        public static readonly string LogBackupFile = "Rocket.{0}.log";
        public static readonly string PermissionFile = "Permissions.config.xml";
        public static readonly string CommandsFile = "Commands.config.xml";
        

        public static readonly string PluginTranslationFileTemplate = "{0}.{1}.translation.xml";
        public static readonly string PluginConfigurationFileTemplate = "{0}.configuration.xml";

        public static readonly string WebConfigurationTemplate = "{0}?configuration={1}&instance={2}";
    }
}
