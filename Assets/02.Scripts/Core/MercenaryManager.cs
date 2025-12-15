using System;
using UnityEngine;
using Data;

namespace Core
{
    public class MercenaryManager : MonoBehaviour
    {
        public static MercenaryManager Instance { get; private set; }

        private const int MAX_LEVEL = 10;
        
        private static readonly int[] _levelUpCosts = { 50, 75, 100, 150, 200, 300, 400, 500, 750 };
        
        public event Action<MercenaryData> OnMercenaryLevelUp;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public int GetLevelUpCost(int currentLevel)
        {
            if (currentLevel >= MAX_LEVEL) return -1;
            if (currentLevel < 1) return -1;

            return _levelUpCosts[currentLevel - 1];
        }
        
        public bool IsMaxLevel(int level)
        {
            return level >= MAX_LEVEL;
        }
        
        public int GetMaxLevel()
        {
            return MAX_LEVEL;
        }
        
        public bool TryLevelUp(MercenaryData mercenary)
        {
            if (mercenary == null) return false;

            int cost = GetLevelUpCost(mercenary.Level);
            if (cost < 0) return false;

            if (PlayerResourceManager.Instance == null) return false;

            if (!PlayerResourceManager.Instance.SpendGold(cost))
            {
                return false;
            }

            mercenary.Level++;
            
            PlayerResourceManager.Instance.SaveMercenaries();

            OnMercenaryLevelUp?.Invoke(mercenary);

            Debug.Log($"[MercenaryManager] {mercenary.DisplayName} 레벨업! Lv.{mercenary.Level - 1} → Lv.{mercenary.Level}");

            return true;
        }
        
        public bool CanLevelUp(MercenaryData mercenary)
        {
            if (mercenary == null) return false;

            int cost = GetLevelUpCost(mercenary.Level);
            if (cost < 0) return false;

            if (PlayerResourceManager.Instance == null) return false;

            return PlayerResourceManager.Instance.HasEnoughGold(cost);
        }
    }
}
