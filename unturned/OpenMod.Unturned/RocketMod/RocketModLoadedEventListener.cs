using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.RocketMod.Events;
using SDG.Unturned;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Unturned.RocketMod
{
    public class RocketModLoadedEventListener : IEventListener<RocketModReadyEvent>
    {
        private static readonly FieldInfo m_DelegatesField;

        static RocketModLoadedEventListener()
        {
            m_DelegatesField = typeof(MulticastDelegate).GetField("delegates", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [EventListener]
        public Task HandleEventAsync(object sender, RocketModReadyEvent @event)
        {
            RemoveRocketCommandListeners();
            return Task.CompletedTask;
        }

        private void RemoveRocketCommandListeners()
        {
            var onCommandWindowInputted = CommandWindow.onCommandWindowInputted;

            var newInvocationArray = onCommandWindowInputted.GetInvocationList()
                .Where(d => !d.GetMethodInfo().Name.Equals("<bindDelegates>b__16_0"))
                .ToArray();

            m_DelegatesField.SetValue(onCommandWindowInputted, newInvocationArray);
        }
    }
}