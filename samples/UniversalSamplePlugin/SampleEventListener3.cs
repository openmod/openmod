using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;
using OpenMod.Core.Eventing;

namespace UniversalSamplePlugin
{
    // This event listener will never fire because SampleEventListener1 cancels the event and this listener does not ignore cancelled events
    // If it would execute, it would execute as the 3rd one.
    public class SampleEventListener3 : IEventListener<SampleEvent>
    {
        private readonly ILogger<SampleEventListener3> m_Logger;

        public SampleEventListener3(ILogger<SampleEventListener3> logger)
        {
            m_Logger = logger;
        }

        [EventListener(Priority = Priority.Highest)]
        public async Task HandleEventAsync(object sender, SampleEvent @event)
        {
            m_Logger.LogInformation($"SampleEventListener3 shouldn't be called!");
        }
    }
}