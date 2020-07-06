using OpenMod.API.Ioc;

namespace OpenMod.EntityFrameworkCore
{
    [Service]
    public interface IConnectionStringAccessor
    {
        string GetConnectionString(string name);
    }
}