using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BasicSkillBehavior", menuName = "Game/Skill Behaviors/Basic")]
    public class BasicSkillBehavior : SkillBehavior
    {
        public override void Execute(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            List<BattleUnit> effectTargets)
        {
            PlayAnimation(caster);
        }
    }
}
