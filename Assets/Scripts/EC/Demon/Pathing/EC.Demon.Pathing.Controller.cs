using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.AI;
using State = EC.Demon.Pathing.Patrol.State;

namespace EC.Demon.Pathing
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[RequireComponent(typeof(NavMeshAgent))]
	[AddComponentMenu("EC.Demon.Pathing.Controller")]
	public class Controller : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		[DisableInEditor] [SerializeField] public NavMeshAgent NavMeshAgent;

		[DisableInEditor] [SerializeField] public GameObject Player;

		public Helpers.Events.Channels.GameObjectEC NavBeaconEC;

		public StateType CurrentStateType;

		[SerializeField] public RandomBag<GameObject> NavBeaconsBag = new();

		public Config Config => _eventBus.Config;

		[field: SerializeField]
		public Helpers.StateMachines.IFSMState<StateType, Controller> CurrentState { get; private set; }

		public Dictionary<StateType, Func<Helpers.StateMachines.IFSMState<StateType, Controller>>> States =>
			new()
			{
				{ StateType.Patrol, () => new State().Init(this, Config.PatrolConfig) },
				{ StateType.Chase, () => new Chase.State().Init(this, Config.ChaseConfig) },
			};

		public void Awake()
		{
			NavMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);

			_eventBus = Helpers.Debug.TryFindComponent<EventBus>(gameObject);
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
			Player = Helpers.Debug.TryFindByTag("Player");
			NavMeshAgent.ApplySteeringConfig(Config.SteeringConfig);
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