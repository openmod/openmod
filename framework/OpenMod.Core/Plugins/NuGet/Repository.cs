namespace OpenMod.Core.Plugins.NuGet
{
    public class Repository
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsEnabled { get; set; }
        public string ApiKey { get; set; }
    }
}