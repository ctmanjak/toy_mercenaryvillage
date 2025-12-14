using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Stage_", menuName = "Game/Stage Data")]
    public class StageData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("고유 ID (예: \"1-1\", \"1-2\")")]
        public string StageId;

        [Tooltip("표시용 이름")]
        public string StageName;

        [Header("Enemies")]
        [Tooltip("적 스폰 리스트")]
        public EnemySpawnInfo[] Enemies;

        [Header("Difficulty")]
        [Tooltip("권장 전투력")]
        public int RecommendedPower;

        [Header("Rewards")]
        [Tooltip("승리 시 기본 골드")]
        public int GoldReward;
    }
}
