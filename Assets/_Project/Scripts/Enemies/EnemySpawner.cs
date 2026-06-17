using System.Collections.Generic;
using Lean.Pool;
using TopdownSurvival.Core;
using TopdownSurvival.Level;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace TopdownSurvival.Enemies
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        private const float k_NavSampleDistance = 4f;

        [SerializeField] private EnemyHealth m_EnemyPrefab;
        [SerializeField] private Transform m_Player;
        [SerializeField] private float m_DistanceJitter = 1.5f;

        private readonly HashSet<EnemyHealth> m_Active = new HashSet<EnemyHealth>();
        private GameEventBus m_Bus;
        private float m_WaveInterval = 5f;
        private float m_SpawnDistance = 12f;
        private int m_BatchSize = 12;
        private bool m_Running;
        private float m_NextSpawnTime;

        [Inject]
        public void Construct(GameEventBus bus)
        {
            m_Bus = bus;
        }

        private void Awake()
        {
            if (m_EnemyPrefab == null)
            {
                Debug.LogError($"{nameof(EnemySpawner)} on '{name}' has no enemy prefab.", this);
            }
        }

        public void Configure(LevelData level)
        {
            if (level == null)
            {
                return;
            }

            m_WaveInterval = level.WaveInterval;
            m_SpawnDistance = level.SpawnDistance;
            m_BatchSize = level.EnemiesPerWave;
        }

        public void BeginSpawning()
        {
            m_Running = true;
            m_NextSpawnTime = 0f;
        }

        public void StopSpawning(bool clearAlive)
        {
            m_Running = false;

            if (!clearAlive)
            {
                return;
            }

            foreach (EnemyHealth enemy in m_Active)
            {
                if (enemy != null)
                {
                    enemy.Died -= OnEnemyDied;
                    LeanPool.Despawn(enemy);
                }
            }

            m_Active.Clear();
        }

        private void Update()
        {
            if (!m_Running || m_EnemyPrefab == null || m_Player == null)
            {
                return;
            }

            if (Time.time < m_NextSpawnTime)
            {
                return;
            }

            m_NextSpawnTime = Time.time + m_WaveInterval;
            SpawnBatch();
        }

        private void SpawnBatch()
        {
            int count = m_BatchSize;
            if (count <= 0)
            {
                return;
            }

            float angleStep = 360f / count;
            float startAngle = Random.value * 360f;

            for (int i = 0; i < count; i++)
            {
                float angle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
                float distance = m_SpawnDistance + Random.Range(-m_DistanceJitter, m_DistanceJitter);
                Vector3 candidate = m_Player.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;
                SpawnOne(candidate);
            }
        }

        private void SpawnOne(Vector3 candidate)
        {
            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, k_NavSampleDistance, NavMesh.AllAreas))
            {
                return;
            }

            EnemyHealth enemy = LeanPool.Spawn(m_EnemyPrefab, hit.position, Quaternion.identity);
            if (enemy == null)
            {
                return;
            }

            enemy.Died -= OnEnemyDied;
            enemy.Died += OnEnemyDied;
            m_Active.Add(enemy);

            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Activate(hit.position, m_Player);
            }
        }

        private void OnEnemyDied(EnemyHealth enemy)
        {
            m_Active.Remove(enemy);
            Vector3 position = enemy != null ? enemy.transform.position : Vector3.zero;
            m_Bus?.Raise(new EnemyKilledEvent(position));
        }
    }
}
