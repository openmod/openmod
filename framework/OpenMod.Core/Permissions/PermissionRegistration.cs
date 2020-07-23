using OpenMod.API;
using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    public class PermissionRegistration : IPermissionRegistration
    {
        public IOpenModComponent Owner { get; set; }    

        public string Permission { get; set; }

        public string Description { get; set; }

        public PermissionGrantResult DefaultGrant { get; set; } = PermissionGrantResult.Default;
    }
}