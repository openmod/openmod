using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Ioc;

namespace OpenMod.Core.Commands
{
    [Service]
    public interface ICommandDataStore
    {
        Task<RegisteredCommandsData> GetRegisteredCommandsAsync();
        
        Task<RegisteredCommandData?> GetRegisteredCommandAsync(string commandId);

        [OpenModInternal]
        Task SetRegisteredCommandsAsync(RegisteredCommandsData data);
        
        Task<T?> GetCommandDataAsync<T>(string commandId, string key);
        
        Task SetCommandDataAsync<T>(string commandId, string key, T? value);
        
        Task SetCommandDataAsync(RegisteredCommandData data);
    }
}