using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public static class SkillParamKey
    {
        // 공통
        public const string Damage = "damage";
        public const string HealAmount = "healAmount";
        public const string Duration = "duration";

        // Buff/Debuff
        public const string BuffMultiplier = "buffMultiplier";
        public const string BuffDuration = "buffDuration";

        // Stun
        public const string StunDuration = "stunDuration";

        // Shield
        public const string ShieldAmount = "shieldAmount";
        public const string ShieldDuration = "shieldDuration";
    }

    [Serializable]
    public class SkillParameter
    {
        [Tooltip("파라미터 키 (SkillParamKey 참조)")]
        public string Key;

        [Tooltip("파라미터 값")]
        public float Value;

        public SkillParameter() { }

        public SkillParameter(string key, float value)
        {
            Key = key;
            Value = value;
        }
    }

    public enum TriggerType
    {
        CooldownReady,
        HpBelow,
        HpAbove,
        BattleStart,
        OnKill,
        AllyHpBelow
    }

    public enum EffectType
    {
        None,
        Damage,
        DamageAoE,
        Heal,
        HealAoE,
        BuffAtk,
        DebuffAtk,
        Stun,
        Shield
    }

    public enum TargetType
    {
        NearestEnemy,
        LowestHpEnemy,
        LowestHpPercentageEnemy,
        Self,
        LowestHpAlly,
        LowestHpPercentageAlly,
        AllEnemiesInRange,
        AllAlliesInRange
    }

    public enum EffectOrigin
    {
        Caster,
        Target
    }

    [CreateAssetMenu(fileName = "Skill_", menuName = "Game/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("기본 정보")]
        [Tooltip("고유 ID")]
        public string SkillId;

        [Tooltip("스킬 이름")]
        public string SkillName;

        [Tooltip("스킬 설명")]
        [TextArea(2, 4)]
        public string Description;

        [Header("발동 조건")]
        [Tooltip("스킬 쿨다운 (초)")]
        public float Cooldown = 5f;

        [Tooltip("발동 조건 타입")]
        public TriggerType TriggerType;

        [Tooltip("발동 조건 값 (예: HP 30% 이하면 0.3)")]
        [Range(0f, 1f)]
        public float TriggerValue;

        [Header("효과")]
        [Tooltip("효과 타입")]
        public EffectType EffectType;

        [Tooltip("효과 적용 횟수 (애니메이션 이벤트 횟수와 맞춰야 함)")]
        public int HitCount = 1;

        [Header("타겟팅")]
        [Tooltip("타겟 선택 방식")]
        public TargetType TargetType;

        [Tooltip("스킬 사거리")]
        public float Range = 5f;

        [Header("범위 효과")]
        [Tooltip("효과 기준점 (Caster: 시전자 중심, Target: 1차 타겟 중심)")]
        public EffectOrigin EffectOrigin = EffectOrigin.Caster;

        [Tooltip("범위 효과 반경 (0이면 단일 대상)")]
        public float EffectRadius = 0f;

        [Header("투사체")]
        [Tooltip("스킬용 투사체 (null이면 유닛 기본 투사체 사용)")]
        public ProjectileData ProjectileData;

        [Tooltip("유닛의 기본 투사체 사용 여부")]
        public bool UseUnitProjectile;

        [Header("비주얼")]
        [Tooltip("스킬 아이콘")]
        public Sprite Icon;

        [Tooltip("스킬 이펙트 프리팹")]
        public GameObject EffectPrefab;

        [Tooltip("이펙트 지속 시간 (초)")]
        public float EffectDuration = 1f;

        [Header("파라미터")]
        [Tooltip("스킬별 커스텀 파라미터")]
        public List<SkillParameter> Parameters = new List<SkillParameter>();

        [Header("특수 동작")]
        [Tooltip("특수 동작 (위치 이동 등). null이면 기본 EffectType 동작만 실행")]
        public SkillBehavior CustomBehavior;
        
        public float GetParam(string key, float defaultValue = 0f)
        {
            foreach (var param in Parameters)
            {
                if (param.Key == key)
                    return param.Value;
            }
            return defaultValue;
        }
        
        public bool HasParam(string key)
        {
            foreach (var param in Parameters)
            {
                if (param.Key == key)
                    return true;
            }
            return false;
        }

    }
}
