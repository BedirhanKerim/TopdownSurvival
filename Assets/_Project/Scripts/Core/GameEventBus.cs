using GenericEventBus;

namespace TopdownSurvival.Core
{
    public sealed class GameEventBus : GenericEventBus<IGameEvent>
    {
    }
}
