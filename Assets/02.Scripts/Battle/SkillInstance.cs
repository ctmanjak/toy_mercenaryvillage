using Data;
using UnityEngine;

namespace Battle
{
    [System.Serializable]
    public class SkillInstance
    {
        [SerializeField] private SkillData _data;
        [SerializeField] private float _currentCooldown;

        public SkillData Data => _data;
        public float CurrentCooldown => _currentCooldown;
        public float MaxCooldown => _data != null ? _data.Cooldown : 0f;
        public bool IsReady => _currentCooldown <= 0f;
        public float CooldownRatio => MaxCooldown > 0f ? _currentCooldown / MaxCooldown : 0f;

        public SkillInstance(SkillData data)
        {
            _data = data;
            _currentCooldown = 0f;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (_currentCooldown > 0f)
            {
                _currentCooldown -= deltaTime;
                if (_currentCooldown < 0f)
                {
                    _currentCooldown = 0f;
                }
            }
        }

        public void ResetCooldown()
        {
            _currentCooldown = MaxCooldown;
        }
        
        public bool CheckTriggerCondition(BattleUnit owner)
        {
            if (_data == null || owner == null) return false;

            switch (_data.TriggerType)
            {
                case TriggerType.CooldownReady:
                    return IsReady;

                case TriggerType.HpBelow:
                    return IsReady && owner.HealthRatio <= _data.TriggerValue;

                case TriggerType.HpAbove:
                    return IsReady && owner.HealthRatio >= _data.TriggerValue;

                case TriggerType.BattleStart:
                    return false;

                case TriggerType.OnKill:
                    // TODO: OnKill은 별도 이벤트로 처리
                    return false;

                case TriggerType.AllyHpBelow:
                    // TODO: AllyHpBelow는 별도 로직 필요
                    return false;

                default:
                    return false;
            }
        }
    }
}
