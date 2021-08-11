namespace OpenMod.API.Eventing
{
    public interface IEventListenerOptions
    {
        EventListenerPriority Priority { get; }

        bool IgnoreCancelled { get; }
    }
}
