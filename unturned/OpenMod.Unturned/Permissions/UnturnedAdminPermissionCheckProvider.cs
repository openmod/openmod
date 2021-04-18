using Microsoft.Extensions.Configuration;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Configuration;
using OpenMod.Unturned.Users;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Permissions
{
    [Priority(Priority = Priority.High)]
    public class UnturnedAdminPermissionCheckProvider : IPermissionCheckProvider
    {
        private readonly IConfiguration m_Configuration;

        public UnturnedAdminPermissionCheckProvider(IOpenModUnturnedConfiguration configuration)
        {
            m_Configuration = configuration.Configuration;
        }

        public bool SupportsActor(IPermissionActor actor)
        {
            return actor is UnturnedUser user && user.Player.SteamPlayer.isAdmin;
        }

        public Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            return Task.FromResult(m_Configuration.GetValue("grantAdminsAllPerms", defaultValue: true)
                ? PermissionGrantResult.Grant
                : PermissionGrantResult.Default);
        }
    }
}
