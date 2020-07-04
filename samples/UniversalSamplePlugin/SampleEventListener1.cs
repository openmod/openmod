using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;
using OpenMod.Core.Eventing;

namespace UniversalSamplePlugin
{
    // This event listener gets executed first because it has a lower priority than the other ones
    public class SampleEventListener1 : IEventListener<SampleEvent>
    {
        private readonly ILogger<SampleEventListener1> m_Logger;
        private int m_Count; // This will be always 0 because the event listener will be always recreated as it is not set as singleton

        public SampleEventListener1(ILogger<SampleEventListener1> logger)
        {
            m_Logger = logger;
        }

        [EventListener(Priority = Priority.Low)]
        public async Task HandleEventAsync(object sender, SampleEvent @event)
        {
            m_Logger.LogInformation($"SampleEventListener1: {@event.Value}, count: {++m_Count}");
            @event.Value++;
            @event.IsCancelled = true;
        }
    }
}