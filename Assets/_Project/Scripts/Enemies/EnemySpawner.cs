using Lean.Pool;
using UnityEngine;
using UnityEngine.AI;

namespace TopdownSurvival.Enemies
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        private const float k_NavSampleDistance = 4f;

        [SerializeField] private EnemyHealth m_EnemyPrefab;
        [SerializeField] private Transform m_Player;
        [SerializeField] private float m_SpawnInterval = 1.5f;
        [SerializeField] private float m_SpawnRadius = 15f;
        [SerializeField] private int m_MaxAlive = 20;

        private int m_AliveCount;
        private float m_NextSpawnTime;

        private void Awake()
        {
            if (m_EnemyPrefab == null)
            {
                Debug.LogError($"{nameof(EnemySpawner)} on '{name}' has no enemy prefab.", this);
            }
        }

        private void Update()
        {
            if (m_EnemyPrefab == null || m_Player == null)
            {
                return;
            }

            if (m_AliveCount >= m_MaxAlive || Time.time < m_NextSpawnTime)
            {
                return;
            }

            m_NextSpawnTime = Time.time + m_SpawnInterval;
            SpawnOne();
        }

        private void SpawnOne()
        {
            if (!TryGetSpawnPosition(out Vector3 position))
            {
                return;
            }

            EnemyHealth enemy = LeanPool.Spawn(m_EnemyPrefab, position, Quaternion.identity);
            if (enemy == null)
            {
                return;
            }

            enemy.Died -= OnEnemyDied;
            enemy.Died += OnEnemyDied;

            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Activate(position, m_Player);
            }

            m_AliveCount++;
        }

        private bool TryGetSpawnPosition(out Vector3 position)
        {
            Vector2 circle = Random.insideUnitCircle * m_SpawnRadius;
            Vector3 candidate = m_Player.position + new Vector3(circle.x, 0f, circle.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, k_NavSampleDistance, NavMesh.AllAreas))
            {
                position = hit.position;
                return true;
            }

            position = candidate;
            return false;
        }

        private void OnEnemyDied(EnemyHealth enemy)
        {
            m_AliveCount--;
        }
    }
}
