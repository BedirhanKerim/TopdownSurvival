using DG.Tweening;
using TMPro;
using TopdownSurvival.Core;
using TopdownSurvival.Level;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace TopdownSurvival.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TMP_Text m_TimerText;
        [SerializeField] private TMP_Text m_KillText;

        [Header("Game Won")]
        [SerializeField] private GameObject m_WonPanel;
        [SerializeField] private TMP_Text m_WonScoreText;
        [SerializeField] private Button m_NextButton;

        [Header("Game Over")]
        [SerializeField] private GameObject m_OverPanel;
        [SerializeField] private TMP_Text m_OverScoreText;
        [SerializeField] private Button m_RetryButton;

        private GameEventBus m_Bus;
        private LevelManager m_Level;
        private int m_LastSecond = -1;
        private int m_LastKills = -1;

        [Inject]
        public void Construct(GameEventBus bus, LevelManager level)
        {
            m_Bus = bus;
            m_Level = level;
            m_Bus.SubscribeTo<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void Awake()
        {
            if (m_NextButton != null)
            {
                m_NextButton.onClick.AddListener(OnNextClicked);
            }

            if (m_RetryButton != null)
            {
                m_RetryButton.onClick.AddListener(OnRetryClicked);
            }

            if (m_WonPanel != null)
            {
                m_WonPanel.SetActive(false);
            }

            if (m_OverPanel != null)
            {
                m_OverPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (m_Bus != null)
            {
                m_Bus.UnsubscribeFrom<EnemyKilledEvent>(OnEnemyKilled);
            }
        }

        private void Update()
        {
            if (m_Level == null)
            {
                return;
            }

            if (m_TimerText != null)
            {
                int seconds = Mathf.CeilToInt(Mathf.Max(0f, m_Level.RemainingTime));
                if (seconds != m_LastSecond)
                {
                    m_LastSecond = seconds;
                    m_TimerText.text = $"{seconds / 60:00}:{seconds % 60:00}";
                }
            }

            if (m_KillText != null && m_Level.Kills != m_LastKills)
            {
                m_LastKills = m_Level.Kills;
                m_KillText.text = m_LastKills.ToString();
            }
        }

        public void ShowGameWon(int kills, int total)
        {
            if (m_WonScoreText != null)
            {
                m_WonScoreText.text = $"Defeated: {kills}\nTotal: {total}";
            }

            if (m_WonPanel != null)
            {
                m_WonPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }

        public void ShowGameOver(int kills, int total)
        {
            if (m_OverScoreText != null)
            {
                m_OverScoreText.text = $"Defeated: {kills}\nTotal: {total}";
            }

            if (m_OverPanel != null)
            {
                m_OverPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }

        public void HideResults()
        {
            if (m_WonPanel != null)
            {
                m_WonPanel.SetActive(false);
            }

            if (m_OverPanel != null)
            {
                m_OverPanel.SetActive(false);
            }

            Time.timeScale = 1f;
        }

        private void OnNextClicked()
        {
            m_Bus?.Raise(new NextLevelRequestedEvent());
        }

        private void OnRetryClicked()
        {
            m_Bus?.Raise(new RetryRequestedEvent());
        }

        private void OnEnemyKilled(ref EnemyKilledEvent e)
        {
            if (m_KillText == null)
            {
                return;
            }

            Transform t = m_KillText.transform;
            t.DOKill(true);
            t.localScale = Vector3.one;
            t.DOPunchScale(Vector3.one * 0.3f, 0.2f, 8, 0.8f);
        }
    }
}
