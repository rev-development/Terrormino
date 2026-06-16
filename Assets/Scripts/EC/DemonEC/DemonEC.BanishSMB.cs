using UnityEngine;

namespace EC.DemonEC
{
    public class BanishSMB : StateMachineBehaviour
    {

        [Helpers.DisableInEditor]
        private EventBus
            _eventBus; // Do not assign in Editor, it will create a global reference to a single Demon's component

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            _eventBus = Helpers.Debug.TryFindComponentInParent<EventBus>(animator.gameObject);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (_eventBus) _eventBus.BanishFxCompleted.Invoke();
        }

    }
}