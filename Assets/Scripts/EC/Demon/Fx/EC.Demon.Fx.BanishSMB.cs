using Helpers.Ext;
using UnityEngine;

namespace EC.Demon.Fx
{
	public class BanishSMB : StateMachineBehaviour
	{
		[Helpers.DisableInEditorAttribute]
		private EventBus
			_eventBus; // Do not assign in Editor, it will create a global reference to a single Demon's component

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
			_eventBus = animator.gameObject.TryFindComponentInParent<EventBus>();

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_eventBus) _eventBus.BanishFxCompleted.Invoke();
		}
	}
}