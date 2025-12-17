using System;
using Battle;
using Data;
using UnityEngine;

namespace Core
{
    public class ExpeditionManager : MonoBehaviour
    {
        public static ExpeditionManager Instance { get; private set; }

        [Header("State")]
        [SerializeField] private ExpeditionData _currentExpedition;
        [SerializeField] private int _currentBattleIndex;
        [SerializeField] private int _totalGoldEarned;
        [SerializeField] private bool _isExpeditionActive;

        public ExpeditionData CurrentExpedition => _currentExpedition;
        public int CurrentBattleIndex => _currentBattleIndex;
        public int TotalGoldEarned => _totalGoldEarned;
        public bool IsExpeditionActive => _isExpeditionActive;
        public int TotalBattles => _currentExpedition?.Battles?.Count ?? 0;
        public bool IsLastBattle => _currentBattleIndex >= TotalBattles - 1;

        public event Action<ExpeditionData, int> OnExpeditionComplete;
        public event Action<ExpeditionData, int> OnExpeditionFailed;
        public event Action<int, int, int> OnBattleComplete; // battleIndex, gold, totalGold

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnBattleEnd += HandleBattleEnd;
            }
        }

        private void OnDisable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnBattleEnd -= HandleBattleEnd;
            }
        }

        public void StartExpedition(ExpeditionData expedition)
        {
            if (expedition == null)
            {
                Debug.LogError("[ExpeditionManager] ExpeditionData is null");
                return;
            }

            if (expedition.Battles == null || expedition.Battles.Count == 0)
            {
                Debug.LogError("[ExpeditionManager] Expedition has no battles");
                return;
            }

            _currentExpedition = expedition;
            _currentBattleIndex = 0;
            _totalGoldEarned = 0;
            _isExpeditionActive = true;

            Debug.Log($"[ExpeditionManager] Starting expedition: {expedition.ExpeditionName}");
            LoadCurrentBattle();
        }

        public void LoadCurrentBattle()
        {
            if (!_isExpeditionActive || _currentExpedition == null)
            {
                Debug.LogWarning("[ExpeditionManager] No active expedition");
                return;
            }

            if (_currentBattleIndex >= _currentExpedition.Battles.Count)
            {
                Debug.LogWarning("[ExpeditionManager] All battles completed");
                return;
            }

            var stage = _currentExpedition.Battles[_currentBattleIndex];
            if (stage == null)
            {
                Debug.LogError($"[ExpeditionManager] Stage at index {_currentBattleIndex} is null");
                return;
            }

            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.StartBattle(stage);
                Debug.Log($"[ExpeditionManager] Loading battle {_currentBattleIndex + 1}/{TotalBattles}: {stage.StageName}");
            }
            else
            {
                Debug.LogError("[ExpeditionManager] BattleManager.Instance is null");
            }
        }

        public void LoadNextBattle()
        {
            if (!_isExpeditionActive)
            {
                Debug.LogWarning("[ExpeditionManager] No active expedition");
                return;
            }

            _currentBattleIndex++;

            if (_currentBattleIndex >= TotalBattles)
            {
                CompleteExpedition();
                return;
            }

            LoadCurrentBattle();
        }

        private void HandleBattleEnd(bool victory, int goldReward)
        {
            if (!_isExpeditionActive) return;

            if (victory)
            {
                _totalGoldEarned += goldReward;
                OnBattleComplete?.Invoke(_currentBattleIndex, goldReward, _totalGoldEarned);

                Debug.Log($"[ExpeditionManager] Battle {_currentBattleIndex + 1} won! Gold: +{goldReward} (Total: {_totalGoldEarned})");

                if (IsLastBattle)
                {
                    CompleteExpedition();
                }
            }
            else
            {
                FailExpedition();
            }
        }

        private void CompleteExpedition()
        {
            if (!_isExpeditionActive) return;

            int completionBonus = _currentExpedition.CompletionBonus;
            int totalReward = _totalGoldEarned + completionBonus;

            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.AddGold(totalReward);
            }

            Debug.Log($"[ExpeditionManager] Expedition complete! Total gold: {totalReward} (Battles: {_totalGoldEarned} + Bonus: {completionBonus})");

            var completedExpedition = _currentExpedition;
            _isExpeditionActive = false;

            OnExpeditionComplete?.Invoke(completedExpedition, totalReward);
        }

        private void FailExpedition()
        {
            if (!_isExpeditionActive) return;

            if (PlayerResourceManager.Instance != null && _totalGoldEarned > 0)
            {
                PlayerResourceManager.Instance.AddGold(_totalGoldEarned);
            }

            Debug.Log($"[ExpeditionManager] Expedition failed at battle {_currentBattleIndex + 1}. Gold earned: {_totalGoldEarned}");

            var failedExpedition = _currentExpedition;
            int earnedGold = _totalGoldEarned;
            _isExpeditionActive = false;

            OnExpeditionFailed?.Invoke(failedExpedition, earnedGold);
        }

        public void ReturnToTown()
        {
            if (!_isExpeditionActive)
            {
                Debug.LogWarning("[ExpeditionManager] No active expedition to return from");
                return;
            }

            if (PlayerResourceManager.Instance != null && _totalGoldEarned > 0)
            {
                PlayerResourceManager.Instance.AddGold(_totalGoldEarned);
            }

            Debug.Log($"[ExpeditionManager] Returning to town with {_totalGoldEarned} gold");

            _isExpeditionActive = false;
            _currentExpedition = null;
        }

        public StageData GetCurrentStage()
        {
            if (!_isExpeditionActive || _currentExpedition == null)
                return null;

            if (_currentBattleIndex >= 0 && _currentBattleIndex < _currentExpedition.Battles.Count)
                return _currentExpedition.Battles[_currentBattleIndex];

            return null;
        }
    }
}
