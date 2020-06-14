using OpenMod.API.Console;
using OpenMod.Core.Permissions;

namespace OpenMod.Core.Console
{
    public class ConsolePermissionProvider : AlwaysGrantPermissionCheckProvider
    {
        public ConsolePermissionProvider() : base(actor => actor is IConsoleActor)
        {
        }
    }
}