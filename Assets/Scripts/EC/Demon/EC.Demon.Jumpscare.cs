using Helpers.Attributes;
using Helpers.Ext;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[AddComponentMenu("EC.Demon.Jumpscare")]
	public class Jumpscare : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		[SerializeField] private float _jumpscareFrameFitPercent = 0.8f;

		// [DisableInEditor] [SerializeField] private ControlPanel _controlPanel;

		private Camera _mainCamera;

		private NavMeshAgent _navMeshAgent;

		[UsedImplicitly]
		public void Awake()
		{
			_eventBus = gameObject.TryFindComponent<EventBus>();

			// _controlPanel = gameObject.TryFindComponent<ControlPanel>();

			_navMeshAgent = gameObject.TryFindComponent<NavMeshAgent>();

			if (Camera.main != null) _mainCamera = Camera.main;
		}

		private void OnEnable() => _eventBus.JumpscareTriggered.AddListener(PrepJumpscare);

		// if (_controlPanel)
		// 	_controlPanel.ListenerTracker.Add(this, nameof(_eventBus.JumpscareTriggered), nameof(PrepJumpscare));
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player")) _eventBus.JumpscareTriggered.Invoke(other.gameObject);
		}

		private void RemoveJumpscareListeners() => _eventBus.JumpscareTriggered.RemoveListener(PositionForJumpscare);

		// if (_controlPanel)
		// _controlPanel.ListenerTracker.Remove(this, nameof(_eventBus.JumpscareTriggered), nameof(PositionForJumpscare));
		public void PositionForJumpscare(GameObject player)
		{
			var bounds = gameObject.TryFindComponentsInChildren<Collider>().GetAllBounds();

			var newPosition = _mainCamera.GetPointInFrustum(
				_mainCamera.GetDistanceToFitInFrame(bounds, _jumpscareFrameFitPercent)
			);

			newPosition.y -= gameObject.TryFindComponentsInChildren<Collider>().GetAllBounds().extents.y;

			gameObject.transform.position = newPosition;

			var newRotation = Quaternion.LookRotation(-_mainCamera.transform.forward, _mainCamera.transform.up);
			gameObject.transform.rotation = newRotation;
		}

		public void PrepJumpscare(GameObject player)
		{
			RemoveJumpscareListeners();
			_navMeshAgent.StopResetDisable();
			PositionForJumpscare(player);
		}
	}
}