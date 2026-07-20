using Helpers.Ext;
using UnityEngine;

namespace EC.Demon.Fx
{
	public class JumpscareSMB : StateMachineBehaviour
	{
		public AudioClip Scream;

		[Helpers.DisableInEditorAttribute]
		private AudioSource
			_audioSource; // Do not assign in Editor, it will create a global reference to a single Demon's component

		[Helpers.DisableInEditorAttribute]
		private EventBus
			_eventBus; // Do not assign in Editor, it will create a global reference to a single Demon's component

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_audioSource = animator.gameObject.TryFindComponent<AudioSource>();
			_eventBus = animator.gameObject.TryFindComponentInParent<EventBus>();

			if (_audioSource && Scream) _audioSource.PlayOneShot(Scream);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_eventBus) _eventBus.JumpscareFxCompleted.Invoke();
		}
	}
}