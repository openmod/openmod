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
    public class UserEventsListener : IEventListener<UserConnectedEvent>, IEventListener<UserDisconnectedEvent>
    {
        private readonly UserDatabaseDbContext m_DbContext;

        public UserEventsListener(UserDatabaseDbContext dbContext)
        {
            m_DbContext = dbContext;
        }
        public Task HandleEventAsync(object sender, UserConnectedEvent @event)
        {
            return OnUserEvent(@event.User, "connect");
        }

        public Task HandleEventAsync(object sender, UserDisconnectedEvent @event)
        {
            return OnUserEvent(@event.User, "disconnect");
        }

        private async Task OnUserEvent(IUser user, string @event)
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

            await m_DbContext.AddAsync(new UserActivity
            {
                Date = DateTime.UtcNow,
                Type = @event,
                UserId = user.Id
            });
            await m_DbContext.SaveChangesAsync();
        }
    }
}