namespace OpenMod.API
{
    public class RuntimeInitParameters
    {
        public string WorkingDirectory { get; set; } = string.Empty;
        public string[] CommandlineArgs { get; set; } = new string[0];
        public object? PackageManager { get; set; }
    }
}