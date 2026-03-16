using OpenMod.API.Ioc;
using System;
using System.Threading.Tasks;

namespace OpenMod.API.Persistence
{
    [Service]
    public interface IOpenModSerializer
    {
        Task<T?> DeserializeAsync<T>(ReadOnlyMemory<byte> memory);

        Task<ReadOnlyMemory<byte>> SerializeAsync<T>(T dataObject);
    }
}
