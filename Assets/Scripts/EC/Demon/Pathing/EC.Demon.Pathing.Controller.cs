using System;
using System.Collections.Generic;
using Helpers.Ext;
using JetBrains.Annotations;
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
		[Helpers.DisableInEditorAttribute] [SerializeField] private EventBus _eventBus;

		[Helpers.DisableInEditorAttribute] [SerializeField] public NavMeshAgent NavMeshAgent;

		[Helpers.DisableInEditorAttribute] [SerializeField] public GameObject Player;

		public Helpers.Events.Channels.GameObjectEC NavBeaconEC;

		[PublicAPI] public StateType CurrentStateType; // For inspector

		[SerializeField] public Helpers.RandomBag<GameObject> NavBeaconsBag = new();

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
			Player = Helpers.TryFind.ByTag("Player");
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