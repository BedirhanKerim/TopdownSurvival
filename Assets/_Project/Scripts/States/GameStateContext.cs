namespace TopdownSurvival.States
{
    public sealed class GameStateContext
    {
        public GameStateMachine Machine;
        public IGameState Playing;
        public IGameState GameWon;
        public IGameState GameOver;
        public IGameState Transition;
        public int NextLevelIndex;
    }
}
