using JetBrains.Annotations;

namespace OpenMod.API
{
    public class RuntimeInitParameters
    {
        public string WorkingDirectory;
        public string[] CommandlineArgs;

        [CanBeNull]
        public object PackageManager;
    }
}