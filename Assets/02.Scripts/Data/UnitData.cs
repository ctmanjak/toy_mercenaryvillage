using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Data
{
    public enum UnitRole { Tank, Damage, Support }
    public enum AttackType { Melee, Bow, Staff }

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

        [Tooltip("공격 타입")]
        public AttackType AttackType;

        [Tooltip("유닛 프리팹")]
        public BattleUnit Prefab;

        [Header("Base Stats")]
        [Tooltip("기본 생명력")]
        public float BaseHealth = 100f;

        [Tooltip("기본 공격력")]
        public float BaseAttackDamage = 10f;

        [Tooltip("기본 공격 속도")]
        public float BaseAttackSpeed = 1f;

        [Tooltip("기본 이동 속도")]
        public float BaseMoveSpeed = 3f;

        [Tooltip("기본 사거리")]
        public float BaseAttackRange = 1.5f;

        [Header("Projectile")]
        [Tooltip("투사체 데이터 (null이면 근거리 공격)")]
        public ProjectileData ProjectileData;

        [Header("Skills")]
        [Tooltip("보유 스킬 목록 (MVP에서는 1개만 사용)")]
        public List<SkillData> Skills = new List<SkillData>();

        public float GetHealth(int level)
            => BaseHealth * GetLevelMultiplier(level);

        public float GetAttackDamage(int level)
            => BaseAttackDamage * GetLevelMultiplier(level);

        private float GetLevelMultiplier(int level)
            => 1f + (level - 1) * 0.1f;

        public int GetCombatPower(int level = 1)
        {
            return (int)(GetHealth(level) + GetAttackDamage(level) * 5);
        }
    }
}
