namespace TopdownSurvival.Core
{
    /// <summary>
    /// Marker interface for every event raised through the global event bus
    /// (<see cref="GenericEventBus.GenericEventBus{TBaseEvent}"/>).
    /// Implement it on small <c>struct</c>s (preferred) so messages are passed
    /// by <c>ref</c> with zero heap allocation.
    /// </summary>
    public interface IGameEvent
    {
    }
}
