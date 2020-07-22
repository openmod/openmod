using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Permissions
{
    [Service]
    public interface IPermissionRegistry
    {
        void RegisterPermission(
            [NotNull] IOpenModComponent component, 
            [NotNull] string permission, 
            string description = null, 
            PermissionGrantResult? defaultGrant = null);

        IReadOnlyCollection<IPermissionRegistration> GetPermissions(IOpenModComponent component);

        [CanBeNull]
        IPermissionRegistration FindPermission(string permission);

        [CanBeNull]
        IPermissionRegistration FindPermission(IOpenModComponent component, string permission);
    }
}