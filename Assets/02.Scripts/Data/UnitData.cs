using Battle;
using UnityEngine;

namespace Data
{
    public enum UnitRole { Tank, Damage, Support }

    [CreateAssetMenu(fileName = "Unit_", menuName = "Game/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Info")]
        [Tooltip("고유 ID")]
        public string UnitId;

        [Tooltip("표시용 이름")]
        public string UnitName;

        [Tooltip("직업")]
        public UnitRole Role;

        [Tooltip("유닛 프리팹")]
        public BattleUnit Prefab;

        [Header("Base Stats")]
        [Tooltip("기본 HP")]
        public float BaseHealth = 100f;

        [Tooltip("기본 공격력")]
        public float BaseAttackDamage = 10f;

        [Tooltip("기본 공격 속도")]
        public float BaseAttackSpeed = 1f;

        [Tooltip("기본 이동 속도")]
        public float BaseMoveSpeed = 3f;

        [Tooltip("기본 사거리")]
        public float BaseAttackRange = 1.5f;

        public float GetHealth(int level)
            => BaseHealth * GetLevelMultiplier(level);

        public float GetAttackDamage(int level)
            => BaseAttackDamage * GetLevelMultiplier(level);

        private float GetLevelMultiplier(int level)
            => 1f + (level - 1) * 0.1f;
    }
}
