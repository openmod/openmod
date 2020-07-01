using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;
using OpenMod.Core.Eventing;

namespace UniversalSamplePlugin
{
    // This event listener will be executed after SampleEventListener1 because this event listeners priority is higher
    [EventListenerLifetime(ServiceLifetime.Singleton)]
    public class SampleEventListener2 : IEventListener<SampleEvent>
    {
        private readonly ILogger<SampleEventListener2> m_Logger;
        private int m_Count; // This will keep incrementing because the event listener is set as singleton

        public SampleEventListener2(ILogger<SampleEventListener2> logger)
        {
            m_Logger = logger;
        }

        [EventListener(Priority = Priority.High, IgnoreCancelled =  true)]
        public async Task HandleEventAsync(object emitter, SampleEvent @event)
        {
            m_Logger.LogInformation($"SampleEventListener2: {@event.Value}, count: {++m_Count}");
        }
    }
}