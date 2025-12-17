using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Expedition_", menuName = "Game/Expedition Data")]
    public class ExpeditionData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("고유 ID (예: \"prairie\", \"forest\")")]
        public string ExpeditionId;

        [Tooltip("표시용 이름")]
        public string ExpeditionName;

        [TextArea(2, 4)]
        [Tooltip("원정 설명")]
        public string Description;

        [Tooltip("원정 아이콘")]
        public Sprite Icon;

        [Header("Difficulty")]
        [Tooltip("권장 전투력")]
        public int RecommendedPower;

        [Header("Battles")]
        [Tooltip("원정 전투 목록 (순서대로 진행)")]
        public List<StageData> Battles = new List<StageData>();

        [Header("Rewards")]
        [Tooltip("완료 시 추가 골드")]
        public int CompletionBonus;

        [Tooltip("첫 완료 보너스 (1회성)")]
        public int FirstClearBonus;

        [Header("Unlock")]
        [Tooltip("해금 조건 (null이면 기본 해금)")]
        public ExpeditionData UnlockRequirement;
    }
}
