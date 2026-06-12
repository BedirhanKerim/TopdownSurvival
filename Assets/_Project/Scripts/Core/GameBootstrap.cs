using VContainer;
using VContainer.Unity;

namespace TopdownSurvival.Core
{
    /// <summary>
    /// Root composition root for the game. Every cross-system service is
    /// registered here and constructor-injected into its consumers by
    /// VContainer, so concrete types never reach for each other directly.
    /// Attach this component to a persistent GameObject in the Boot scene;
    /// it survives scene loads as the parent scope for child LifetimeScopes.
    /// </summary>
    public sealed class GameBootstrap : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Services are registered here as each system is built:
            //   --- Core ---                     (event bus, etc.)
            //   --- Save / persistence ---       (SaveSystem)
            //   --- Level system ---             (LevelManager)
            //   --- Gameplay / state machine --- (GameStateMachine)
        }
    }
}
