using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    [Service]
    public interface IEntityDirectory
    {
        Task<IReadOnlyCollection<IEntity>> GetEntitiesAsync();

        Task<IReadOnlyCollection<IEntityAsset>> GetEntityAssetsAsync();
    }
}