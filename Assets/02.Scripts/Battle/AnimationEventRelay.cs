using UnityEngine;

namespace Battle
{
    public class AnimationEventRelay : MonoBehaviour
    {
        private UnitAnimator _unitAnimator;

        public void Initialize(UnitAnimator unitAnimator)
        {
            _unitAnimator = unitAnimator;
        }
        
        public void AnimEvent_AttackHit()
        {
            _unitAnimator?.AnimEvent_AttackHit();
        }

        public void AnimEvent_AttackEnd()
        {
            _unitAnimator?.AnimEvent_AttackEnd();
        }

        public void AnimEvent_SkillHit()
        {
            _unitAnimator?.AnimEvent_SkillHit();
        }

        public void AnimEvent_DeathComplete()
        {
            _unitAnimator?.AnimEvent_DeathComplete();
        }
    }
}
