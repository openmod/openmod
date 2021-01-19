using System;
using System.Diagnostics.CodeAnalysis;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.RocketMod.Events;
using SDG.Unturned;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#pragma warning disable IDE0079 // Remove unnecessary suppression
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

        [SuppressMessage("ReSharper", "DelegateSubtraction")]
        private void RemoveRocketCommandListeners()
        {
            var commandWindowInputedInvocationList = CommandWindow.onCommandWindowInputted.GetInvocationList();
            foreach (var @delegate in commandWindowInputedInvocationList
                .Where(IsRocketModDelegate))
            {
                CommandWindow.onCommandWindowInputted -= (CommandWindowInputted)@delegate;
            }

            var checkPermissionsList = ChatManager.onCheckPermissions.GetInvocationList();
            foreach (var @delegate in checkPermissionsList
                .Where(IsRocketModDelegate))
            {
                ChatManager.onCheckPermissions -= (CheckPermissions)@delegate;
            }
        }

        private bool IsRocketModDelegate(Delegate @delegate)
        {
            var methodInfo = @delegate.GetMethodInfo();
            var assembly = methodInfo?.DeclaringType?.Assembly;
            return RocketModIntegration.IsRocketModAssembly(assembly);
        }
    }
}