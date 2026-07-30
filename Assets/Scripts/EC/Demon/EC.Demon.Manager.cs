using System.Collections.Generic;
using Helpers;
using Helpers.Attributes;
using Helpers.Events.Channels;
using Helpers.Ext;
using UnityEngine;
using UnityEngine.AI;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	[AddComponentMenu("EC.Demon.Manager")]
	public class Manager : SingletonMonoBehaviour<Manager>
	{
		[SerializeField] private GameObject _demonPrefab;

		[SerializeField] private GameObjectEC _removeDemon;

		[SerializeField] private VoidEC _gameOver;

		[NavMeshAreaMask] [SerializeField] private int _spawnAreaMask = 1; // 0 is always Nothing, 1 is always Walkable

		[SerializeField] private List<Collider> _spawnColliders = new();

		[DisableInEditor] [SerializeField] private List<GameObject> _demons = new();

		[SerializeField] private Timer _graceTimer = new();

		[SerializeField] private Timer _spawnTimer = new();

		[field: SerializeField] public ConfigSO ConfigSO { get; private set; }

		private int _startFrame;

		public void OnEnable()
		{
			_removeDemon.OnEventRaised += OnRemoveDemon;
			_gameOver.OnEventRaised += OnGameOver;
		}

		private void Start()
		{
			_startFrame = Time.frameCount;
			gameObject.CheckIfEmptyListInInspector(_spawnColliders, "Spawn Colliders");
			_graceTimer.Init(ConfigSO.SpawnGracePeriod);
			_spawnTimer.Init(ConfigSO.SpawnInterval);
		}

		public void Update()
		{
			_graceTimer.Tick(Time.deltaTime);
			_spawnTimer.Tick(Time.deltaTime);

			if (Time.frameCount != _startFrame
				&& !_graceTimer.Dirty)
				_graceTimer.StartNewTimer();

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
			if (_demons.Count == ConfigSO.DemonMax) return;

			if (_spawnColliders.Count == 0) return;

			// 1. Pick one of X colliders to build bounds from
			var selectedSpawnCollider = _spawnColliders[Random.Range(0, _spawnColliders.Count)];
			// 2. Create mutable bounds
			Bounds spawnBounds = new(selectedSpawnCollider.bounds.center, selectedSpawnCollider.bounds.size);

			// 3. NavMesh.SamplePosition is just a RayCast down that detects NavMesh Areas
			if (!NavMesh.SamplePosition(
					spawnBounds.SampleRandom2DPosition(),
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
				eventBus.ApplyConfig(ConfigSO);

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

		public void ClearAll()
		{
			for (var i = 0; i < _demons.Count; i++) OnRemoveDemon(_demons[i]);
		}

		/// <summary>
		///     Not necessary at the moment, but preventing raw Config assignment is good practice.
		///     Ensures any cascading changes are enforced.
		///     If a GameObject has a hub-style component, only that one needs an assignment function.
		///     The rest can just forward the property (public ConfigType Config => HubComponent.Config).
		/// </summary>
		/// <param name="configSO"></param>
		public void ApplyConfig(ConfigSO configSO) => ConfigSO = configSO;

		public void OnGameOver() => ClearAll();

		public void OnRemoveDemon(GameObject targetGameObject)
		{
			_demons.Remove(targetGameObject);
			Destroy(targetGameObject);
		}
	}
}