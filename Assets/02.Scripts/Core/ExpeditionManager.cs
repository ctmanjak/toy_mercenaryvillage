using System;
using Battle;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public enum ExpeditionEndReason { Complete, Failed, Retreat }

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

        public event Action<ExpeditionData, ExpeditionEndReason, int> OnExpeditionEnd;
        public event Action<int, int, int, int, bool> OnBattleVictory;
        public event Action<int, int, int> OnBattleDefeat;

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

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            UnsubscribeFromBattleManager();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "BattleScene" && _isExpeditionActive)
            {
                Invoke(nameof(SubscribeToBattleManager), 0.1f);
            }
        }

        private void SubscribeToBattleManager()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnBattleEnd += HandleBattleEnd;
                Debug.Log("[ExpeditionManager] Subscribed to BattleManager");
            }
            else
            {
                Debug.LogWarning("[ExpeditionManager] BattleManager.Instance is null");
            }
        }

        private void UnsubscribeFromBattleManager()
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

            Debug.Log($"[ExpeditionManager] Loading battle {_currentBattleIndex + 1}/{TotalBattles}: {stage.StageName}");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartBattle(stage);
            }
            else
            {
                Debug.LogError("[ExpeditionManager] GameManager.Instance is null");
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

            UnsubscribeFromBattleManager();

            if (victory)
            {
                _totalGoldEarned += goldReward;

                Debug.Log($"[ExpeditionManager] Battle {_currentBattleIndex + 1} won! Gold: +{goldReward} (Total: {_totalGoldEarned})");

                OnBattleVictory?.Invoke(_currentBattleIndex, TotalBattles, goldReward, _totalGoldEarned, IsLastBattle);
            }
            else
            {
                Debug.Log($"[ExpeditionManager] Battle {_currentBattleIndex + 1} lost.");
                OnBattleDefeat?.Invoke(_currentBattleIndex, TotalBattles, _totalGoldEarned);
            }
        }

        public void CompleteExpedition() => EndExpedition(ExpeditionEndReason.Complete);
        public void FailExpedition() => EndExpedition(ExpeditionEndReason.Failed);
        public void ReturnToTown() => EndExpedition(ExpeditionEndReason.Retreat);

        private void EndExpedition(ExpeditionEndReason reason)
        {
            if (!_isExpeditionActive) return;

            int totalGold = _totalGoldEarned;
            
            if (reason == ExpeditionEndReason.Complete)
            {
                totalGold += _currentExpedition.CompletionBonus;

                if (ExpeditionProgressManager.Instance != null)
                {
                    ExpeditionProgressManager.Instance.RecordCompletion(_currentExpedition.ExpeditionId);

                    if (ExpeditionProgressManager.Instance.TryClaimFirstClearBonus(_currentExpedition, out int bonus))
                    {
                        totalGold += bonus;
                    }
                }
            }
            
            if (totalGold > 0)
            {
                PlayerResourceManager.Instance?.AddGold(totalGold);
            }

            Debug.Log($"[ExpeditionManager] Expedition ended: {reason}, Gold: {totalGold}");

            var expedition = _currentExpedition;
            _isExpeditionActive = false;
            _currentExpedition = null;

            OnExpeditionEnd?.Invoke(expedition, reason, totalGold);

            GameManager.Instance?.GoToTown();
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
