using OpenMod.API;
using OpenMod.API.Commands;
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Commands
{
    [OpenModInternal]
    public class UnturnedCommandSource : ICommandSource
    {
        private readonly List<ICommandRegistration> m_Commands;

        // ReSharper disable once SuggestBaseTypeForParameter /* we don't want this because of DI */
        public UnturnedCommandSource(IOpenModHost host)
        {
            m_Commands = new List<ICommandRegistration>();

            foreach (var cmd in Commander.commands)
            {
                m_Commands.Add(new UnturnedCommandRegistration(host, cmd));
            }
        }

        public Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync()
        {
            return Task.FromResult<IReadOnlyCollection<ICommandRegistration>>(m_Commands);
        }
    }
}