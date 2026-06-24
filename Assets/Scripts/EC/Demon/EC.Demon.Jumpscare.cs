using UnityEngine;
using UnityEngine.AI;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[AddComponentMenu("EC.Demon.Jumpscare")]
	public class Jumpscare : MonoBehaviour
	{
		[Helpers.DisableInEditorAttribute] [SerializeField] private EventBus _eventBus;

		public float JumpscarePlacementDistance = 4.5f;

		[Helpers.DisableInEditorAttribute] private ControlPanel _controlPanel;

		private Transform _mainCameraTransform;

		private NavMeshAgent _navMeshAgent;

		public void PositionForJumpscare(GameObject player)
		{
			_eventBus.JumpscareTriggered.RemoveListener(PositionForJumpscare);

			if (_controlPanel)
				_controlPanel.ListenerTracker.Remove(
					this,
					nameof(_eventBus.JumpscareTriggered),
					nameof(PositionForJumpscare)
				);

			Helpers.NavMeshAgentExtensions.TogglePathing(_navMeshAgent, false);
			_navMeshAgent.ResetPath();
			_navMeshAgent.enabled = false;

			Helpers.Positioning.PositionInFrontOf(
				gameObject.transform,
				_mainCameraTransform,
				JumpscarePlacementDistance
			);

			Helpers.Positioning.AlignTops(gameObject, _mainCameraTransform.gameObject);

			var newRotation = Quaternion.LookRotation(-_mainCameraTransform.forward, _mainCameraTransform.up);
			gameObject.transform.rotation = newRotation;
		}

#region Event Functions

		public void Awake()
		{
			_eventBus = Helpers.Debug.TryFindComponent<EventBus>(gameObject);
			_controlPanel = Helpers.Debug.TryFindComponent<ControlPanel>(gameObject);

			_navMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);

			if (Camera.main != null) _mainCameraTransform = Camera.main.transform;
		}

		private void OnEnable()
		{
			_eventBus.JumpscareTriggered.AddListener(PositionForJumpscare);

			if (_controlPanel)
				_controlPanel.ListenerTracker.Add(
					this,
					nameof(_eventBus.JumpscareTriggered),
					nameof(PositionForJumpscare)
				);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player")) _eventBus.JumpscareTriggered.Invoke(other.gameObject);
		}

#endregion
	}
}