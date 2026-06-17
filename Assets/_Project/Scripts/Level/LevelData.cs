using UnityEngine;

namespace TopdownSurvival.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "TopdownSurvival/Level Data")]
    public sealed class LevelData : ScriptableObject
    {
        [SerializeField] private float m_SurvivalDuration = 180f;
        [SerializeField] private float m_WaveInterval = 6f;
        [SerializeField] private int m_EnemiesPerWave = 8;
        [SerializeField] private float m_SpawnDistance = 14f;

        public float SurvivalDuration => m_SurvivalDuration;
        public float WaveInterval => m_WaveInterval;
        public int EnemiesPerWave => m_EnemiesPerWave;
        public float SpawnDistance => m_SpawnDistance;
    }
}
