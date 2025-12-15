using System;
using Battle;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class MercenaryData
    {
        [Header("Identity")]
        [Tooltip("고유 식별자 (GUID)")]
        public string Id;

        [Tooltip("기반 데이터 (SO 참조)")]
        [NonSerialized] public UnitData UnitData;

        [Tooltip("UnitData ID (저장용)")]
        public string UnitDataId;

        [Tooltip("플레이어 지정 이름 (선택)")]
        public string CustomName;

        [Header("Progression")]
        [Tooltip("현재 레벨 (1~10)")]
        [Range(1, 10)]
        public int Level = 1;

        [Tooltip("현재 경험치 (v0.1에서는 미사용)")]
        public int CurrentExp = 0;

        public MercenaryData(UnitData template)
        {
            Id = Guid.NewGuid().ToString();
            UnitData = template;
            UnitDataId = template.UnitId;
            CustomName = template.UnitName;
            Level = 1;
            CurrentExp = 0;
        }

        public void RestoreUnitData(UnitData unitData)
        {
            UnitData = unitData;
        }
        
        public float GetCurrentHP()
            => UnitData.GetHealth(Level);
        
        public float GetCurrentAttackDamage()
            => UnitData.GetAttackDamage(Level);
        
        public UnitStats GetCurrentStats()
            => UnitStats.FromUnitData(UnitData, Level);
        
        public BattleUnit CreateBattleUnit(Vector3 position, Team team = Team.Ally)
        {
            if (UnitData == null || UnitData.Prefab == null)
            {
                Debug.LogError($"[MercenaryData] UnitData or Prefab is null for mercenary: {CustomName}");
                return null;
            }

            var go = UnityEngine.Object.Instantiate(UnitData.Prefab, position, Quaternion.identity);
            var battleUnit = go.GetComponent<BattleUnit>();

            if (battleUnit == null)
            {
                Debug.LogError($"[MercenaryData] Prefab does not have BattleUnit component: {UnitData.Prefab.name}");
                UnityEngine.Object.Destroy(go);
                return null;
            }

            battleUnit.Initialize(UnitData, Level, team);
            return battleUnit;
        }
        
        public string DisplayName
            => string.IsNullOrEmpty(CustomName) ? UnitData?.UnitName : CustomName;

        private float GetLevelMultiplier()
            => 1f + (Level - 1) * 0.1f;
    }
}
