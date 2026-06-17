namespace TopdownSurvival.States
{
    public sealed class TransitionState : IGameState
    {
        private const float k_Duration = 0.5f;

        private readonly GameStateContext m_Context;
        private float m_Timer;

        public TransitionState(GameStateContext context)
        {
            m_Context = context;
        }

        public void Enter()
        {
            m_Timer = k_Duration;
        }

        public void Tick(float deltaTime)
        {
            m_Timer -= deltaTime;
            if (m_Timer <= 0f)
            {
                m_Context.Machine.ChangeState(m_Context.Playing);
            }
        }

        public void Exit()
        {
        }
    }
}
