using System;
using UnityEngine;

namespace TopdownSurvival.Save
{
    [Serializable]
    public sealed class SaveData
    {
        [SerializeField] private int m_TotalEnemiesDefeated;
        [SerializeField] private int m_HighestUnlockedLevel;

        public int TotalEnemiesDefeated
        {
            get => m_TotalEnemiesDefeated;
            set => m_TotalEnemiesDefeated = value;
        }

        public int HighestUnlockedLevel
        {
            get => m_HighestUnlockedLevel;
            set => m_HighestUnlockedLevel = value;
        }
    }
}
