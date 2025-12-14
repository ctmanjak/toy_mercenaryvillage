using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        [Tooltip("적 유닛 기본 데이터")]
        public UnitData UnitData;

        [Tooltip("적 레벨")]
        public int Level = 1;

        [Tooltip("스폰 위치 인덱스 (0~3)")]
        [Range(0, 3)]
        public int SpawnIndex;
    }
}
