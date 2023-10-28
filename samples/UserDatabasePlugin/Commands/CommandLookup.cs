using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using UserDatabasePlugin.Database;

namespace UserDatabasePlugin.Commands
{
    [Command("lookup")]
    [CommandDescription("Looks up a player")]
    [CommandSyntax("<player name or id>")]
    [RegisterCommandPermission(OfflineLookupPermission, Description = "Allows to look up offline players")]
    public class CommandLookup : Command
    {
        public const string OfflineLookupPermission = "offline";

        private readonly IUserManager m_UserManager;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly UserDatabaseDbContext m_DbContext;

        public CommandLookup(IServiceProvider serviceProvider,
            IUserManager userManager,
            IStringLocalizer stringLocalizer,
            UserDatabaseDbContext dbContext) : base(serviceProvider)
        {
            m_UserManager = userManager;
            m_StringLocalizer = stringLocalizer;
            m_DbContext = dbContext;
        }

        protected override async Task OnExecuteAsync()
        {
            var userNameOrId = await Context.Parameters.GetAsync<string>(0);
            var user = (await m_UserManager.FindUserAsync(KnownActorTypes.Player, userNameOrId, UserSearchMode.FindByNameOrId));

            var userId = user?.Id;
            var lastJoinTime = user?.Session?.SessionStartTime;
            var lastUsername = user?.DisplayName;

            if (lastJoinTime == null && await CheckPermissionAsync(OfflineLookupPermission) == PermissionGrantResult.Grant)
            {
                var storedUser = await m_DbContext.Users
                    .FirstOrDefaultAsync(d => d.Id == userNameOrId);

                if (storedUser == null)
                {
                    storedUser = (await m_DbContext.UserActivities
                        .OrderByDescending(d => d.Date)
                        .FirstOrDefaultAsync(d => d.UserName == userNameOrId))?.User;
                }

                if (storedUser != null)
                {
                    var userActivity = (await m_DbContext.UserActivities
                        .OrderByDescending(d => d.Date)
                        .FirstAsync(d => d.UserId == storedUser.Id));

                    userId = userActivity.UserId;
                    lastJoinTime = userActivity.Date;
                    lastUsername = userActivity.UserName;
                }
            }

            if (lastJoinTime == null)
            {
                throw new UserFriendlyException(m_StringLocalizer["user_not_found", new { User = userNameOrId }]!);
            }

            await PrintAsync($"Id: {userId}");
            await PrintAsync($"Last Name: {lastUsername}");
            await PrintAsync($"Last Seen: {lastJoinTime}");
        }
    }
}