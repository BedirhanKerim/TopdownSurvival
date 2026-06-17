using TopdownSurvival.Core;
using TopdownSurvival.Level;

namespace TopdownSurvival.States
{
    public sealed class PlayingState : IGameState
    {
        private readonly GameStateContext m_Context;
        private readonly GameEventBus m_Bus;
        private readonly LevelManager m_Level;

        public PlayingState(GameStateContext context, GameEventBus bus, LevelManager level)
        {
            m_Context = context;
            m_Bus = bus;
            m_Level = level;
        }

        public void Enter()
        {
            m_Bus.SubscribeTo<LevelCompletedEvent>(OnLevelCompleted);
            m_Bus.SubscribeTo<PlayerDiedEvent>(OnPlayerDied);
            m_Level.StartLevel(m_Context.NextLevelIndex);
        }

        public void Tick(float deltaTime)
        {
            m_Level.Tick(deltaTime);
        }

        public void Exit()
        {
            m_Bus.UnsubscribeFrom<LevelCompletedEvent>(OnLevelCompleted);
            m_Bus.UnsubscribeFrom<PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnLevelCompleted(ref LevelCompletedEvent e)
        {
            m_Context.Machine.ChangeState(m_Context.GameWon);
        }

        private void OnPlayerDied(ref PlayerDiedEvent e)
        {
            m_Context.Machine.ChangeState(m_Context.GameOver);
        }
    }
}
