using OpenMod.Core.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Console
{
    public class ConsolePermissionProvider : AlwaysGrantPermissionCheckProvider
    {
        public ConsolePermissionProvider() : base(actor => actor.Type == KnownActorTypes.Console)
        {
        }
    }
}