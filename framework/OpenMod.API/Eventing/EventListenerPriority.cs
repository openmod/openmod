namespace OpenMod.API.Eventing
{
    /// <summary>
    /// Represents the priority of an event listener. The invocation order is based on from lowest to highest.
    /// </summary>
    public enum EventListenerPriority
    {
        /// <summary>
        /// The lowest priority. Lowest priority event listeners are executed first.
        /// </summary>
        Lowest,
        /// <summary>
        /// The low priority.
        /// </summary>
        Low,
        /// <summary>
        /// The normal priority. This is the default value if no priority is set.
        /// </summary>
        Normal,
        /// <summary>
        /// The high priority.
        /// </summary>
        High,
        /// <summary>
        /// The highest priority. 
        /// </summary>
        Highest,
        /// <summary>
        /// The monitor priority.
        /// Monitor priority event listeners are called last and must not change event state.
        /// They can also not cancel or uncancel events.
        /// </summary>
        Monitor
    }
}
