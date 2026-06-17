using System.IO;
using UnityEngine;

namespace TopdownSurvival.Save
{
    public sealed class SaveSystem
    {
        private const string k_FileName = "save.json";

        private readonly string m_Path;
        private SaveData m_Data = new SaveData();

        public SaveData Data => m_Data;

        public SaveSystem()
        {
            m_Path = Path.Combine(Application.persistentDataPath, k_FileName);
        }

        public void Load()
        {
            if (!File.Exists(m_Path))
            {
                m_Data = new SaveData();
                return;
            }

            string json = File.ReadAllText(m_Path);
            m_Data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(m_Data, true);
            File.WriteAllText(m_Path, json);
        }

        public void AddKills(int amount)
        {
            m_Data.TotalEnemiesDefeated += amount;
        }

        public void UnlockLevel(int index)
        {
            if (index > m_Data.HighestUnlockedLevel)
            {
                m_Data.HighestUnlockedLevel = index;
            }
        }
    }
}
