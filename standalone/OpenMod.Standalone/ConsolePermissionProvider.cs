using OpenMod.API.Permissions;
using OpenMod.Core.Permissions;

namespace OpenMod.Standalone
{
    public class ConsolePermissionProvider : AlwaysGrantPermissionCheckProvider
    {
        public ConsolePermissionProvider() : base(actor => actor is ConsoleActor)
        {
        }
    }
}