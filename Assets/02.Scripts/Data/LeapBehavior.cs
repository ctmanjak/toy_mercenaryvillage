using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "LeapBehavior", menuName = "Game/Skill Behaviors/Leap")]
    public class LeapBehavior : SkillBehavior
    {
        public override bool HandlesCompletion => true;

        [Header("도약 설정")]
        [Tooltip("타겟과의 정지 거리")]
        public float StopDistance = 1.5f;

        [Tooltip("도약 시간")]
        public float Duration = 0.5f;

        [Tooltip("도약 높이")]
        public float JumpHeight = 2f;

        [Tooltip("도약 애니메이션 설정")]
        public LeapSettings LeapSettings;

        public override void Execute(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            List<BattleUnit> effectTargets)
        {
            if (caster == null || primaryTarget == null)
            {
                caster?.OnSkillComplete();
                return;
            }

            caster.Leap(primaryTarget, StopDistance, Duration, JumpHeight, LeapSettings, caster.OnSkillComplete);

            Debug.Log($"[LeapBehavior] {caster.name} leaping to {primaryTarget.name}");
        }
    }
}
