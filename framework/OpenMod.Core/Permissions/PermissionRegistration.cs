namespace OpenMod.API.Permissions
{
    public class PermissionRegistration : IPermissionRegistration
    {
        public IOpenModComponent Owner { get; set; }    

        public string Permission { get; set; }

        public string Description { get; set; }

        public PermissionGrantResult DefaultGrant { get; set; } = PermissionGrantResult.Default;
    }
}