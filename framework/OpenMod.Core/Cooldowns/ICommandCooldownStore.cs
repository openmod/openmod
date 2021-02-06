using System;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;

namespace OpenMod.Core.Cooldowns
{
    [Service]
    public interface ICommandCooldownStore
    {
        Task<TimeSpan?> GetCooldownSpanAsync(ICommandActor actor, string commandId);

        Task<DateTime?> GetLastExecutedAsync(ICommandActor actor, string command);

        Task RecordExecutionAsync(ICommandActor actor, string command, DateTime time);
    }
}