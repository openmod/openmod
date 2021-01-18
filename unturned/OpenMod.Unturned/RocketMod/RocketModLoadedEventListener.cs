using System;
using System.Linq;
using System.Reflection;
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
                .Where(d => !d.GetMethodInfo().Name.Equals("<bindDelegates>b__16_0"))
                .ToList();

            var invocationListField = onCommandWindowInputted.GetType().GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            var invocationListCountField = onCommandWindowInputted.GetType().GetField("_invocationCount", BindingFlags.NonPublic | BindingFlags.Instance);
            invocationListField.SetValue(onCommandWindowInputted, new object[] { newInvocationList });
            invocationListCountField.SetValue(onCommandWindowInputted, (IntPtr)newInvocationList.Count);
        }
    }
}