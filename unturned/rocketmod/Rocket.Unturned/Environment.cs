using System;
using System.IO;

namespace Rocket.Unturned
{
    public static class Environment
    {
        public static string RocketDirectory;
        public static void Initialize()
        {
            RocketDirectory = String.Format("Servers/{0}/Rocket/", U.Instance.InstanceId);
            if (!Directory.Exists(RocketDirectory)) Directory.CreateDirectory(RocketDirectory);

            Directory.SetCurrentDirectory(RocketDirectory);
        }

        public static readonly string SettingsFile = "Rocket.Unturned.config.xml";
        public static readonly string TranslationFile = "Rocket.Unturned.{0}.translation.xml";
        public static readonly string ConsoleFile = "{0}.console";
    }
}