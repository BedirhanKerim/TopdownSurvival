using TopdownSurvival.Core;
using TopdownSurvival.Enemies;
using TopdownSurvival.Player;
using TopdownSurvival.Save;
using UnityEngine;
using VContainer;

namespace TopdownSurvival.Level
{
    public sealed class LevelManager
    {
        private readonly GameEventBus m_Bus;
        private readonly SaveSystem m_Save;
        private readonly EnemySpawner m_Spawner;
        private readonly PlayerHealth m_Player;
        private readonly LevelData[] m_Levels;

        private int m_CurrentIndex;
        private float m_RemainingTime;
        private int m_Kills;
        private bool m_Running;

        public int CurrentIndex => m_CurrentIndex;
        public int LevelCount => m_Levels != null ? m_Levels.Length : 0;
        public float RemainingTime => m_RemainingTime;
        public int Kills => m_Kills;
        public bool HasNext => m_CurrentIndex + 1 < LevelCount;

        [Inject]
        public LevelManager(GameEventBus bus, SaveSystem save, EnemySpawner spawner, PlayerHealth player, LevelData[] levels)
        {
            m_Bus = bus;
            m_Save = save;
            m_Spawner = spawner;
            m_Player = player;
            m_Levels = levels;

            m_Bus.SubscribeTo<EnemyKilledEvent>(OnEnemyKilled);
        }

        public void StartLevel(int index)
        {
            if (m_Levels == null || index < 0 || index >= m_Levels.Length)
            {
                Debug.LogError($"{nameof(LevelManager)}: invalid level index {index}.");
                return;
            }

            m_CurrentIndex = index;
            LevelData level = m_Levels[index];
            m_RemainingTime = level.SurvivalDuration;
            m_Kills = 0;
            m_Running = true;

            if (m_Player != null)
            {
                m_Player.Revive();
            }

            m_Spawner.Configure(level);
            m_Spawner.BeginSpawning();

            m_Bus.Raise(new LevelStartedEvent(index, m_Levels.Length));
        }

        public void Stop()
        {
            m_Running = false;
            m_Spawner.StopSpawning(true);
        }

        public void Tick(float deltaTime)
        {
            if (!m_Running)
            {
                return;
            }

            m_RemainingTime -= deltaTime;
            if (m_RemainingTime > 0f)
            {
                return;
            }

            m_RemainingTime = 0f;
            Complete();
        }

        private void Complete()
        {
            m_Running = false;
            m_Spawner.StopSpawning(true);

            m_Save.AddKills(m_Kills);
            if (HasNext)
            {
                m_Save.UnlockLevel(m_CurrentIndex + 1);
            }
            m_Save.Save();

            m_Bus.Raise(new LevelCompletedEvent(m_CurrentIndex, m_Kills, m_Save.Data.TotalEnemiesDefeated, HasNext));
        }

        private void OnEnemyKilled(ref EnemyKilledEvent e)
        {
            if (m_Running)
            {
                m_Kills++;
            }
        }
    }
}
