using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "RapidFireBehavior", menuName = "Game/Skill Behaviors/RapidFire")]
    public class RapidFireBehavior : SkillBehavior
    {
        [Header("속사 설정")]
        [Tooltip("공격 횟수")]
        public int AttackCount = 3;

        [Tooltip("애니메이션 속도 배율")]
        public float SpeedMultiplier = 1.5f;

        public override bool HandlesCompletion => true;

        public override void Execute(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            List<BattleUnit> effectTargets)
        {
            caster.UnitAnimator.PlayRepeatedAttack(
                caster.AttackType,
                AttackCount,
                SpeedMultiplier,
                caster.OnSkillComplete
            );
        }
    }
}
