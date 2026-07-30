using System;
using System.Collections.Generic;
using Helpers;
using Helpers.Events.Channels;
using Helpers.Ext;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace EC.Demon.Pathing
{
	[Serializable]
	public enum StateType
	{
		Patrol,

		Chase,
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[RequireComponent(typeof(NavMeshAgent))]
	[AddComponentMenu("EC.Demon.Pathing.Controller")]
	public class Controller : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		[DisableInEditor] [SerializeField] public NavMeshAgent NavMeshAgent;

		[DisableInEditor] [SerializeField] public GameObject Player;

		public GameObjectEC NavBeaconEC;

		[PublicAPI] public StateType CurrentStateType; // For inspector

		[SerializeField] public RandomBag<GameObject> NavBeaconsBag = new();

		public ConfigSO ConfigSO => _eventBus.ConfigSO;

		[field: SerializeField] public IFSMState<StateType, Controller> CurrentState { get; private set; }

		public Dictionary<StateType, Func<IFSMState<StateType, Controller>>> States =>
			new()
			{
				{ StateType.Patrol, () => new EC.Demon.Pathing.Patrol.State().Init(this, ConfigSO.PatrolConfigSO) },
				{ StateType.Chase, () => new EC.Demon.Pathing.Chase.State().Init(this, ConfigSO.ChaseConfigSO) },
			};

		public void Awake()
		{
			NavMeshAgent = gameObject.TryFindComponent<NavMeshAgent>();

			_eventBus = gameObject.TryFindComponent<EventBus>();
		}

		public void OnEnable()
		{
			_eventBus.Illuminated.AddListener(OnIlluminated);
			_eventBus.BanishTriggered.AddListener(OnBanishTriggered);
			NavBeaconsBag.Init(NavBeaconEC.CollectedParams);

			NavBeaconEC.OnEventRaised += OnNewNavBeacon;
		}

		public void Start()
		{
			Player = TryFind.ByTag("Player");
			NavMeshAgent.ApplySteeringConfig(ConfigSO.SteeringConfig);
			EnterState(NavBeaconsBag.HasItems ? StateType.Patrol : StateType.Chase);
		}

		private void Update() => CurrentState.Update();

		public void OnDisable()
		{
			_eventBus.Illuminated.RemoveListener(OnIlluminated);
			_eventBus.BanishTriggered.RemoveListener(OnBanishTriggered);
			NavBeaconEC.OnEventRaised -= OnNewNavBeacon;
		}

		public void EnterState(StateType newStateType)
		{
			CurrentState?.Exit();

			if (!States.ContainsKey(newStateType)) return;

			CurrentStateType = newStateType;

			CurrentState = States[newStateType]();

			CurrentState?.Start();
		}

		private void OnBanishTriggered(GameObject _) => NavMeshAgent.TogglePathing(false);

		private void OnIlluminated(bool isIlluminated) => NavMeshAgent.TogglePathing(!isIlluminated);

		private void OnNewNavBeacon(GameObject navBeacon) => NavBeaconsBag.AddItem(navBeacon);
	}
}