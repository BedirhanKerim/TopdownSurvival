using TopdownSurvival.Save;

namespace TopdownSurvival.States
{
    public sealed class BootState : IGameState
    {
        private readonly GameStateContext m_Context;
        private readonly SaveSystem m_Save;

        public BootState(GameStateContext context, SaveSystem save)
        {
            m_Context = context;
            m_Save = save;
        }

        public void Enter()
        {
            m_Save.Load();
            m_Context.NextLevelIndex = 0;
            m_Context.Machine.ChangeState(m_Context.Playing);
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
