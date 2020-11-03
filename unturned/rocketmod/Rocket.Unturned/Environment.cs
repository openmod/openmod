using System.IO;

namespace Rocket.Unturned
{
    public static class Environment
    {
        public static string RocketDirectory;
        public static void Initialize()
        {
            if (RocketDirectory != null)
                return;

            RocketDirectory = $"Servers/{U.Instance.InstanceId}/Rocket/";
            if (!Directory.Exists(RocketDirectory)) Directory.CreateDirectory(RocketDirectory);

            Directory.SetCurrentDirectory(RocketDirectory);
        }

        public static readonly string SettingsFile = "Rocket.Unturned.config.xml";
        public static readonly string TranslationFile = "Rocket.Unturned.{0}.translation.xml";
        public static readonly string ConsoleFile = "{0}.console";
    }
}