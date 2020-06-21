using OpenMod.API.Prioritization;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Console
{
    [Priority(Priority = Priority.Highest)]
    public class ConsolePermissionProvider : AlwaysGrantPermissionCheckProvider
    {
        public ConsolePermissionProvider() : base(actor => actor.Type == KnownActorTypes.Console)
        {
        }
    }
}