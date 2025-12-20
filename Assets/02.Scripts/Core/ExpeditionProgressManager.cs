using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Core
{
    public class ExpeditionProgressManager : MonoBehaviour
    {
        public static ExpeditionProgressManager Instance { get; private set; }

        private Dictionary<string, ExpeditionProgress> _progressDict = new();

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

        public void LoadFromSaveData(List<ExpeditionProgress> progressList)
        {
            _progressDict.Clear();
            if (progressList == null) return;

            foreach (var progress in progressList)
            {
                if (!string.IsNullOrEmpty(progress.ExpeditionId))
                {
                    _progressDict[progress.ExpeditionId] = progress;
                }
            }
        }

        public List<ExpeditionProgress> GetAllProgress()
        {
            return new List<ExpeditionProgress>(_progressDict.Values);
        }

        public ExpeditionProgress GetProgress(string expeditionId)
        {
            if (_progressDict.TryGetValue(expeditionId, out var progress))
            {
                return progress;
            }
            return null;
        }

        public ExpeditionProgress GetOrCreateProgress(string expeditionId)
        {
            if (!_progressDict.TryGetValue(expeditionId, out var progress))
            {
                progress = new ExpeditionProgress(expeditionId);
                _progressDict[expeditionId] = progress;
            }
            return progress;
        }

        public bool IsUnlocked(ExpeditionData expedition)
        {
            if (expedition == null) return false;

            if (expedition.UnlockRequirement == null)
            {
                return true;
            }

            var requiredProgress = GetProgress(expedition.UnlockRequirement.ExpeditionId);
            return requiredProgress != null && requiredProgress.IsFirstCleared;
        }

        public void RecordCompletion(string expeditionId)
        {
            if (string.IsNullOrEmpty(expeditionId)) return;

            var progress = GetOrCreateProgress(expeditionId);
            progress.CompletionCount++;

            Debug.Log($"[ExpeditionProgressManager] Expedition {expeditionId} completed! Count: {progress.CompletionCount}");
            
            SaveManager.Instance?.SaveGame();
        }

        public void RecordCompletion(ExpeditionData expedition)
        {
            if (expedition == null) return;
            RecordCompletion(expedition.ExpeditionId);
        }

        public bool TryClaimFirstClearBonus(ExpeditionData expedition, out int bonus)
        {
            bonus = 0;
            if (expedition == null)
            {
                Debug.LogWarning("[ExpeditionProgressManager] TryClaimFirstClearBonus: expedition is null");
                return false;
            }

            if (string.IsNullOrEmpty(expedition.ExpeditionId))
            {
                Debug.LogWarning($"[ExpeditionProgressManager] TryClaimFirstClearBonus: ExpeditionId is empty for {expedition.ExpeditionName}");
                return false;
            }

            var progress = GetProgress(expedition.ExpeditionId);
            if (progress == null)
            {
                Debug.LogWarning($"[ExpeditionProgressManager] TryClaimFirstClearBonus: progress is null for {expedition.ExpeditionId}");
                return false;
            }

            if (!progress.CanClaimFirstClearBonus)
            {
                Debug.Log($"[ExpeditionProgressManager] TryClaimFirstClearBonus: Cannot claim. CompletionCount={progress.CompletionCount}, FirstClearClaimed={progress.FirstClearClaimed}");
                return false;
            }

            progress.FirstClearClaimed = true;
            bonus = expedition.FirstClearBonus;

            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.AddGold(bonus);
            }

            Debug.Log($"[ExpeditionProgressManager] First clear bonus claimed: {bonus}G");

            SaveManager.Instance?.SaveGame();

            return true;
        }

        public int GetCompletionCount(string expeditionId)
        {
            var progress = GetProgress(expeditionId);
            return progress?.CompletionCount ?? 0;
        }
        
        public bool WillReceiveFirstClearBonus(string expeditionId)
        {
            var progress = GetProgress(expeditionId);

            if (progress == null)
            {
                Debug.Log($"[ExpeditionProgressManager] WillReceiveFirstClearBonus: No progress for {expeditionId}, returning true");
                return true;
            }

            bool result = progress.CompletionCount == 0 && !progress.FirstClearClaimed;
            Debug.Log($"[ExpeditionProgressManager] WillReceiveFirstClearBonus: {expeditionId} - CompletionCount={progress.CompletionCount}, FirstClearClaimed={progress.FirstClearClaimed}, Result={result}");
            return result;
        }

        public string GetUnlockRequirementText(ExpeditionData expedition)
        {
            if (expedition == null || expedition.UnlockRequirement == null)
            {
                return string.Empty;
            }

            return $"\"{expedition.UnlockRequirement.ExpeditionName}\" 원정 완료 시 해금";
        }
    }
}
