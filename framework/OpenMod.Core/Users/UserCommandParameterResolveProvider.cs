using System;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    [Priority(Priority = Priority.Low)]
    public class UserCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        private readonly IUserManager m_UserManager;
        private readonly string m_DefaultActorType;
        private readonly char m_Separator;

        public UserCommandParameterResolveProvider(
            IUserManager userProvider,
            string defaultActorType = KnownActorTypes.Player,
            char separator = ':')
        {
            m_UserManager = userProvider;
            m_DefaultActorType = defaultActorType;
            m_Separator = separator;
        }

        public bool Supports(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typeof(IUser).IsAssignableFrom(type);
        }

        public async Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            GetSearchOptions(input, out string actorType, out string actorNameOrId);

            return await m_UserManager.FindUserAsync(actorType, actorNameOrId, UserSearchMode.FindByNameOrId);
        }

        private void GetSearchOptions(string input, out string actorType, out string actorNameOrId)
        {
            if (input.Contains(m_Separator) && !input.Trim(m_Separator).Contains(m_Separator))
            {
                actorType = m_DefaultActorType;
                actorNameOrId = input;
                return;
            }

            string[] args = input.Split(m_Separator);

            if (args.Length == 1)
            {
                actorType = m_DefaultActorType;
                actorNameOrId = args[0];
            }
            else
            {
                actorType = args[0];
                actorNameOrId = string.Join(m_Separator.ToString(), args.Skip(1));
            }
        }
    }
}
