using OpenMod.API;
using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    public class PermissionRegistration : IPermissionRegistration
    {
        public IOpenModComponent Owner { get; set; } = null!;    

        public string Permission { get; set; } = null!;

        public string? Description { get; set; }

        public PermissionGrantResult DefaultGrant { get; set; } = PermissionGrantResult.Default;
    }
}