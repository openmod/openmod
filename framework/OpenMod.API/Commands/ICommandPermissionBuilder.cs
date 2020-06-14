using System.Collections.Generic;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    [Service]
    public interface ICommandPermissionBuilder
    {
        string GetPermission(ICommandRegistration registration);
        string GetPermission(ICommandRegistration registration, IEnumerable<ICommandRegistration> commands);
    }
}