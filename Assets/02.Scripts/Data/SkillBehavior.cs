using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Data
{
    public abstract class SkillBehavior : ScriptableObject
    {
        [Header("애니메이션")]
        [Tooltip("애니메이션 트리거 이름 (비워두면 재생 안함)")]
        public string AnimationTrigger;
        
        public virtual bool HandlesCompletion => false;
        
        public abstract void Execute(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            List<BattleUnit> effectTargets);
        
        protected void PlayAnimation(BattleUnit caster)
        {
            if (!string.IsNullOrEmpty(AnimationTrigger))
            {
                caster.UnitAnimator.PlaySkillAnimation(AnimationTrigger);
            }
        }
    }
}
