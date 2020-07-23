using System.Threading.Tasks;

namespace OpenMod.API.Eventing
{
    /// <summary>
    ///     The emit callback for events that have finished and notified all listeners.
    /// </summary>
    /// <param name="event"></param>
    public delegate Task EventExecutedCallback(IEvent @event);
}