using TopdownSurvival.Core;
using TopdownSurvival.Level;
using TopdownSurvival.UI;

namespace TopdownSurvival.States
{
    public sealed class GameOverState : IGameState
    {
        private readonly GameStateContext m_Context;
        private readonly GameEventBus m_Bus;
        private readonly LevelManager m_Level;
        private readonly UIManager m_Ui;

        public GameOverState(GameStateContext context, GameEventBus bus, LevelManager level, UIManager ui)
        {
            m_Context = context;
            m_Bus = bus;
            m_Level = level;
            m_Ui = ui;
        }

        public void Enter()
        {
            m_Level.Fail();
            m_Bus.SubscribeTo<RetryRequestedEvent>(OnRetry);
            m_Ui.ShowGameOver(m_Level.Kills, m_Level.TotalKills);
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
            m_Bus.UnsubscribeFrom<RetryRequestedEvent>(OnRetry);
            m_Ui.HideResults();
        }

        private void OnRetry(ref RetryRequestedEvent e)
        {
            m_Context.NextLevelIndex = m_Level.CurrentIndex;
            m_Context.Machine.ChangeState(m_Context.Transition);
        }
    }
}
