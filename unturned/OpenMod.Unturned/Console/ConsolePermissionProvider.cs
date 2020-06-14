using OpenMod.Core.Permissions;

namespace OpenMod.Unturned.Console
{
    public class ConsolePermissionProvider : AlwaysGrantPermissionCheckProvider
    {
        public ConsolePermissionProvider() : base(actor => actor is ConsoleActor)
        {
        }
    }
}