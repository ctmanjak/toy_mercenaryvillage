using System;

namespace Data
{
    [Serializable]
    public class ExpeditionProgress
    {
        public string ExpeditionId;
        public int CompletionCount;
        public bool FirstClearClaimed;

        public ExpeditionProgress() { }

        public ExpeditionProgress(string expeditionId)
        {
            ExpeditionId = expeditionId;
            CompletionCount = 0;
            FirstClearClaimed = false;
        }

        public bool IsFirstCleared => CompletionCount > 0;
        public bool CanClaimFirstClearBonus => IsFirstCleared && !FirstClearClaimed;
    }
}
