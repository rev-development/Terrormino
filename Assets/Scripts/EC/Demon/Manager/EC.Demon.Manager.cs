using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	[AddComponentMenu("EC.Demon.Manager")]
	public class Manager : Helpers.SingletonMonoBehaviour<Manager>
	{
		[SerializeField] private GameObject _demonPrefab;

		[SerializeField] private Helpers.Events.Channels.GameObjectEC _removeDemon;

		[SerializeField] private Helpers.Events.Channels.VoidEC _gameOver;

		[Helpers.Editor.NavMeshAreaMaskAttribute] [SerializeField]
		private int _spawnAreaMask = 0; // 0 is always Walkable

		[SerializeField] private List<Collider> _spawnColliders = new();

		[Helpers.DisableInEditorAttribute] [SerializeField] private List<GameObject> _demons = new();

		[SerializeField] private Helpers.Timer _graceTimer = new();

		[SerializeField] private Helpers.Timer _spawnTimer = new();

		[field: SerializeField] public Config Config { get; private set; } = new();

		public void OnEnable()
		{
			_removeDemon.OnEventRaised += OnRemoveDemon;
			_gameOver.OnEventRaised += OnGameOver;
		}

		private void Start()
		{
			_graceTimer.Init(Config.SpawnGracePeriod);
			_spawnTimer.Init(Config.SpawnInterval);
		}

		public void Update()
		{
			_graceTimer.Tick(Time.deltaTime);
			_spawnTimer.Tick(Time.deltaTime);

			if (!_graceTimer.Dirty) _graceTimer.StartNewTimer();

			if (_graceTimer.Ringing
				&& !_spawnTimer.Active)
				_spawnTimer.StartNewTimer();

			if (_graceTimer.Ringing
				&& _spawnTimer.Ringing)
			{
				SpawnDemon();
				_spawnTimer.StartNewTimer();
			}
		}

		public void OnDisable()
		{
			_removeDemon.OnEventRaised -= OnRemoveDemon;
			_gameOver.OnEventRaised -= OnGameOver;
		}

		public void SpawnDemon()
		{
			if (_demons.Count != Config.DemonMax) return;

			// 1. Pick one of X colliders to build bounds from
			var selectedSpawnCollider = _spawnColliders[Random.Range(0, _spawnColliders.Count)];
			// 2. Create mutable bounds
			Bounds spawnBounds = new(selectedSpawnCollider.bounds.center, selectedSpawnCollider.bounds.size);
			// 2a. Shrink by the collider size to space away from edges (Commented out because NavMeshes automatically subtract Agent width from Walkable
// 			if (!DemonPrefab.TryGetComponent(out Collider _))
// 			{
// #if UNITY_EDITOR
// 				Debug.LogWarning("DemonPrefab has no Collider component.", gameObject);
// #endif
// 				return;
// 			}
			// spawnBounds.Expand(-demonCollider.bounds.extents);

			// 3. Randomize XZ location within bounds
			Vector3 spawnLocation = new(
				Random.Range(spawnBounds.min.x, spawnBounds.max.x),
				spawnBounds.center.y,
				Random.Range(spawnBounds.min.z, spawnBounds.max.z)
			);

			// 4. NavMesh.SamplePosition is just a RayCast down that detects NavMesh Areas
			if (!NavMesh.SamplePosition(
					spawnLocation,
					out var hit,
					10f,
					_spawnAreaMask
				))
			{
#if UNITY_EDITOR
				Debug.LogError("NavMesh.SamplePosition failed, no valid NavMesh near spawn collider.", gameObject);
#endif
				return;
			}

			// 5. Spawn Demon
			var demon = Instantiate(_demonPrefab, hit.position, Quaternion.identity);

#if UNITY_EDITOR
			Debug.Log($"Spawning demon at {hit.position}", demon);
#endif

			// 6. Pass config to EventBus bc it's a hub-type component
			if (demon.TryGetComponent(out EventBus eventBus))
			{
				eventBus.ApplyConfig(Config);

				_demons.Add(demon);
			}
			else
			{
#if UNITY_EDITOR
				Debug.LogError("Demon spawned without EventBus component, please check DemonPrefab", gameObject);
#endif
				Destroy(demon); // Destroy it because something is wrong with the prefab
			}
		}

		public void ClearAll() => _demons.ForEach(OnRemoveDemon);

		/// <summary>
		///     Not necessary at the moment, but preventing raw Config assignment is good practice.
		///     Ensures any cascading changes are enforced.
		///     If a GameObject has a hub-style component, only that one needs an assignment function.
		///     The rest can just forward the property (public ConfigType Config => HubComponent.Config).
		/// </summary>
		/// <param name="config"></param>
		public void ApplyConfig(Config config) => Config = config;

		public void OnGameOver() => ClearAll();

		public void OnRemoveDemon(GameObject targetGameObject)
		{
			_demons.Remove(targetGameObject);
			Destroy(targetGameObject);
		}
	}
}