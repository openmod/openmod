using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Entities;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Entities
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedEntitySpawner : IEntitySpawner
    {
        public Task<IEntity?> SpawnEntityAsync(Vector3 position, string entityId, IEntityState? state = null)
        {
            return Task.FromException<IEntity?>(new NotSupportedException("Unturned does not support spawning entities"));
        }
    }
}