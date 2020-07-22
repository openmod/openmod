using System.Threading.Tasks;
using JetBrains.Annotations;
using Semver;

namespace OpenMod.API.Plugins
{
    public interface IOpenModPlugin : IOpenModComponent
    {
        string DisplayName { get; }

        [CanBeNull]
        string Author { get; }

        [CanBeNull]
        string Website { get; }

        SemVersion Version { get; }

        Task LoadAsync();

        Task UnloadAsync();
    }
}