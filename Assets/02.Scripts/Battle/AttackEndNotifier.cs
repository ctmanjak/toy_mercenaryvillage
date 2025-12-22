using UnityEngine;

namespace Battle
{
    public class AttackEndNotifier : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var relay = animator.GetComponent<AnimationEventRelay>();
            relay?.AnimEvent_AttackEnd();
        }
    }
}
