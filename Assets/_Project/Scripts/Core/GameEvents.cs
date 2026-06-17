using UnityEngine;

namespace TopdownSurvival.Core
{
    public readonly struct EnemyKilledEvent : IGameEvent
    {
        public readonly Vector3 Position;

        public EnemyKilledEvent(Vector3 position)
        {
            Position = position;
        }
    }

    public readonly struct LevelStartedEvent : IGameEvent
    {
        public readonly int LevelIndex;
        public readonly int TotalLevels;

        public LevelStartedEvent(int levelIndex, int totalLevels)
        {
            LevelIndex = levelIndex;
            TotalLevels = totalLevels;
        }
    }

    public readonly struct LevelCompletedEvent : IGameEvent
    {
        public readonly int LevelIndex;
        public readonly int Kills;
        public readonly int TotalKills;
        public readonly bool HasNext;

        public LevelCompletedEvent(int levelIndex, int kills, int totalKills, bool hasNext)
        {
            LevelIndex = levelIndex;
            Kills = kills;
            TotalKills = totalKills;
            HasNext = hasNext;
        }
    }

    public readonly struct NextLevelRequestedEvent : IGameEvent
    {
    }

    public readonly struct PlayerDiedEvent : IGameEvent
    {
    }

    public readonly struct RetryRequestedEvent : IGameEvent
    {
    }
}
