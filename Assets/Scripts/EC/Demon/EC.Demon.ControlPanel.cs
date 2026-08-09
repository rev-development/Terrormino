using System;
using System.Collections;
using System.Collections.Generic;
using Flashlight;
using Helpers;
using Helpers.Events.Channels;
using Helpers.Ext;
using UnityEngine;
using UnityEngine.AI;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	public class ControlPanel : MonoBehaviour
	{
		public GameObject FlashlightPrefab;

		[DisableInEditor] public GameObject SpawnedFlashlight;

		public GameObject JumpscareTarget;

		[HideInInspector] public EventBus EventBus;

		[HideInInspector] public EC.Demon.Fx.Controller Controller;

		[HideInInspector] public Jumpscare Jumpscare;

		[HideInInspector] public Health Health;

		public GameObjectEC NavBeaconEC;

		public UnityEventPlus ListenerTracker = new();

		[HideInInspector] public EC.Demon.Pathing.Controller Pathing;

		private NavMeshAgent _navMeshAgent;

		public Vector3? CachedPositionPreJumpscare;

		[NonSerialized] public List<GameObject> NavBeacons = new();

		private void Awake() => _navMeshAgent = gameObject.TryFindComponent<NavMeshAgent>();

		public void TogglePathing() => _navMeshAgent.TogglePathing();

		public List<Component> GetInitializedComponents()
		{
			var components = new List<Component>();

			if (string.IsNullOrEmpty(gameObject.scene.name)) return components;

			EventBus = gameObject.GetComponent<EventBus>();
			Controller = gameObject.GetComponentInChildren<EC.Demon.Fx.Controller>();
			Health = gameObject.GetComponent<Health>();
			Jumpscare = gameObject.GetComponent<Jumpscare>();
			Pathing = gameObject.GetComponent<EC.Demon.Pathing.Controller>();

			if (EventBus)
			{
				EventBus.Awake();
				components.Add(EventBus);
			}

			if (Controller)
			{
				Controller.Awake();
				components.Add(Controller);
			}

			if (Health)
			{
				Health.Awake();
				components.Add(Health);
			}

			if (Jumpscare)
			{
				Jumpscare.Awake();
				components.Add(Jumpscare);
			}

			if (Pathing)
			{
				Pathing.Awake();
				components.Add(Pathing);
			}

			if (Application.isPlaying) NavBeacons = NavBeaconEC.CollectedParams;

			return components;
		}

		public void SpawnAndTestFlashlight()
		{
			if (!FlashlightPrefab) return;

			DestroyFlashlight();

			var flashlightSpawnPoint = new Vector3(0, 3, -5);

			SpawnedFlashlight = Instantiate(FlashlightPrefab, transform.position + flashlightSpawnPoint, Quaternion.identity);

			if (SpawnedFlashlight.TryGetComponent<Rigidbody>(out var rb))
			{
				rb.useGravity = false;

				SpawnedFlashlight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

				StartCoroutine(ToggleFlashlightDelay());
			}
			else
			{
				DestroyFlashlight();
			}
		}

		private IEnumerator ToggleFlashlightDelay()
		{
			yield return new WaitForSeconds(1);

			if (!SpawnedFlashlight) yield break;

			if (SpawnedFlashlight.TryGetComponent<Shake>(out var shake))
				shake.FlashlightToggled.Invoke(true);
			else
				DestroyFlashlight();
		}

		public void DestroyFlashlight()
		{
			if (!SpawnedFlashlight) return;

			Destroy(SpawnedFlashlight);
			SpawnedFlashlight = null;
		}

		public void PathToJumpscareTarget()
		{
			if (_navMeshAgent
					&& Camera.main != null
					&& JumpscareTarget)
				Pathing.NavMeshAgent.GoTo(JumpscareTarget.gameObject);
		}

		public void ResetJumpscare()
		{
			gameObject.transform.position = Vector3.zero;

			if (_navMeshAgent) _navMeshAgent.enabled = true;

			EventBus.JumpscareTriggered.AddListener(Jumpscare.PositionForJumpscare);

			ListenerTracker.Add(EventBus, nameof(EventBus.JumpscareTriggered), nameof(Jumpscare.PositionForJumpscare));

			Controller.StopJumpscare();
			_navMeshAgent.ResetPath();
		}

		public void PositionForJumpscare()
		{
			if (JumpscareTarget)
			{
				if (CachedPositionPreJumpscare == null) CachedPositionPreJumpscare = gameObject.transform.position;
				Jumpscare.PositionForJumpscare(JumpscareTarget);
			}
		}

		public void RevertPositionFromJumpscare()
		{
			if (CachedPositionPreJumpscare != null)
			{
				gameObject.transform.position = CachedPositionPreJumpscare.Value;
				CachedPositionPreJumpscare = null;
			}
		}
	}
}