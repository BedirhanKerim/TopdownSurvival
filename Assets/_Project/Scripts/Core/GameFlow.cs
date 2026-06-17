using TopdownSurvival.Level;
using TopdownSurvival.Save;
using TopdownSurvival.States;
using TopdownSurvival.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TopdownSurvival.Core
{
    public sealed class GameFlow : IStartable, ITickable
    {
        private readonly GameStateMachine m_Machine;
        private readonly GameStateContext m_Context;
        private readonly IGameState m_Boot;

        [Inject]
        public GameFlow(GameEventBus bus, SaveSystem save, LevelManager level, UIManager ui)
        {
            m_Machine = new GameStateMachine();
            m_Context = new GameStateContext { Machine = m_Machine };

            m_Context.Playing = new PlayingState(m_Context, bus, level);
            m_Context.GameWon = new GameWonState(m_Context, bus, level, ui);
            m_Context.GameOver = new GameOverState(m_Context, bus, level, ui);
            m_Context.Transition = new TransitionState(m_Context);
            m_Boot = new BootState(m_Context, save);
        }

        public void Start()
        {
            m_Machine.ChangeState(m_Boot);
        }

        public void Tick()
        {
            m_Machine.Tick(Time.deltaTime);
        }
    }
}
