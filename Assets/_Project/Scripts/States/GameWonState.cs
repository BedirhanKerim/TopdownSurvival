using TopdownSurvival.Core;
using TopdownSurvival.Level;
using TopdownSurvival.UI;

namespace TopdownSurvival.States
{
    public sealed class GameWonState : IGameState
    {
        private readonly GameStateContext m_Context;
        private readonly GameEventBus m_Bus;
        private readonly LevelManager m_Level;
        private readonly UIManager m_Ui;

        public GameWonState(GameStateContext context, GameEventBus bus, LevelManager level, UIManager ui)
        {
            m_Context = context;
            m_Bus = bus;
            m_Level = level;
            m_Ui = ui;
        }

        public void Enter()
        {
            m_Bus.SubscribeTo<NextLevelRequestedEvent>(OnNextRequested);
            m_Ui.ShowGameWon();
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
            m_Bus.UnsubscribeFrom<NextLevelRequestedEvent>(OnNextRequested);
            m_Ui.HideResults();
        }

        private void OnNextRequested(ref NextLevelRequestedEvent e)
        {
            m_Context.NextLevelIndex = m_Level.HasNext ? m_Level.CurrentIndex + 1 : 0;
            m_Context.Machine.ChangeState(m_Context.Transition);
        }
    }
}
