using System;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.RocketMod.Events;
using SDG.Unturned;

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
            var onCommandWindowInputtedField = typeof(CommandWindow).GetField("onCommandWindowInputted");
            var onCommandWindowInputted = (CommandWindowInputted) onCommandWindowInputtedField.GetValue(null);

            var newInvocationList = onCommandWindowInputted.GetInvocationList()
                .Where(d => !d.GetType().Assembly.GetName().Name.Equals("Rocket.Unturned"))
                .ToList();

            void Execute(string text, ref bool shouldExecuteCommand)
            {
                foreach (var m in newInvocationList)
                {
                    ((CommandWindowInputted) m)(text, ref shouldExecuteCommand);
                }
            }

            onCommandWindowInputtedField.SetValue(null, (CommandWindowInputted)Execute);
        }
    }
}