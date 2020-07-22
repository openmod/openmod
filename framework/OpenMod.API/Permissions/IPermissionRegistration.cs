namespace OpenMod.API.Permissions
{
    public interface IPermissionRegistration
    {
        IOpenModComponent Owner { get; }

        string Permission { get; }

        string Description { get; }

        PermissionGrantResult DefaultGrant { get; }
    }
}