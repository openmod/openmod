using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Users.Events;
using UserDatabasePlugin.Database;

namespace UserDatabasePlugin
{
    [UsedImplicitly]
    public class UserEventsListener : IEventListener<IUserConnectedEvent>, IEventListener<IUserDisconnectedEvent>
    {
        private readonly UserDatabaseDbContext m_DbContext;

        public UserEventsListener(UserDatabaseDbContext dbContext)
        {
            m_DbContext = dbContext;
        }
        public Task HandleEventAsync(object sender, IUserConnectedEvent @event)
        {
            return HandleUserEvent(@event.User, "connect");
        }

        public Task HandleEventAsync(object sender, IUserDisconnectedEvent @event)
        {
            return HandleUserEvent(@event.User, "disconnect");
        }

        private async Task HandleUserEvent(IUser user, string @eventName)
        {
            var userEntity = await m_DbContext.Users.FirstOrDefaultAsync(d => d.Id.Equals(user.Id));
            if (userEntity == null)
            {
                userEntity = new User
                {
                    Id = user.Id,
                    Type = user.Type
                };

                await m_DbContext.Users.AddAsync(userEntity);
                await m_DbContext.SaveChangesAsync();
            }

            await m_DbContext.UserActivities.AddAsync(new UserActivity
            {
                UserName = user.DisplayName,
                Date = DateTime.UtcNow,
                Type = @eventName,
                UserId = user.Id
            });
            await m_DbContext.SaveChangesAsync();
        }
    }
}