using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.RocketMod.Events;
using SDG.Unturned;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Unturned.RocketMod
{
    public class RocketModLoadedEventListener : IEventListener<RocketModReadyEvent>
    {
        [EventListener]
        public Task HandleEventAsync(object sender, RocketModReadyEvent @event)
        {
            RemoveRocketCommandListeners();
            return Task.CompletedTask;
        }

        private void RemoveRocketCommandListeners()
        {
            var onCommandWindowInputtedList = CommandWindow.onCommandWindowInputted.GetInvocationList();

            CommandWindow.onCommandWindowInputted -= (CommandWindowInputted)onCommandWindowInputtedList
                .First(d => d.GetMethodInfo().Name.Equals("<bindDelegates>b__16_0"));
        }
    }
}