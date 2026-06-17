namespace TopdownSurvival.States
{
    public sealed class GameStateMachine
    {
        private IGameState m_Current;

        public IGameState Current => m_Current;

        public void ChangeState(IGameState next)
        {
            m_Current?.Exit();
            m_Current = next;
            m_Current?.Enter();
        }

        public void Tick(float deltaTime)
        {
            m_Current?.Tick(deltaTime);
        }
    }
}
