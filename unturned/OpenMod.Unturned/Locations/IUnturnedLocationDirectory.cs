using OpenMod.API.Ioc;
using System.Collections.Generic;
using System.Numerics;

namespace OpenMod.Unturned.Locations
{
    [Service]
    public interface IUnturnedLocationDirectory
    {
        IReadOnlyCollection<UnturnedLocation> GetLocations();

        UnturnedLocation? FindLocation(string name, bool exact = true);

        UnturnedLocation? GetNearestLocation(Vector3 position);
    }
}
