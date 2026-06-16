using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EventBus))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Pathing : MonoBehaviour
    {

        [Helpers.DisableInEditor] [SerializeField] private EventBus _eventBus;

        [Helpers.DisableInEditor] [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] public Helpers.Nav.AgentSteeringConfig AgentSteeringConfig = new();

        public Helpers.Events.Channels.GameObjectEC NavBeaconEC;

        public GameObject Player;

        [NonSerialized] public List<GameObject> NavBeacons = new();

        public GameObject Bed { get; set; }

        public void Awake() {
            _navMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);

            AgentSteeringConfig.Apply(_navMeshAgent);

            _eventBus = Helpers.Debug.TryFindComponent<EventBus>(gameObject);
        }

        public void Start() {
            NavBeacons = NavBeaconEC.CollectedParams;
            Player = GameObject.FindGameObjectWithTag("Player");
            Bed = GameObject.FindGameObjectWithTag("Bed");
        }

        public void OnEnable() {
            _eventBus.Illuminated.AddListener(isIlluminated => TogglePathing(!isIlluminated));
            _eventBus.BanishTriggered.AddListener(_ => TogglePathing(false));
            NavBeaconEC.OnEventRaised += OnNewNavBeacon;
        }

        public void OnDisable() {
            NavBeaconEC.OnEventRaised -= OnNewNavBeacon;
        }

        private void OnNewNavBeacon(GameObject navBeacon) {
            NavBeacons.Add(navBeacon);
        }

        private void TogglePathing(bool pathingEnabled) {
            Helpers.Nav.TogglePathing(_navMeshAgent, pathingEnabled);
        }

        public void GoTo(GameObject targetGO) {
            Helpers.Nav.TogglePathing(_navMeshAgent, true);
            _navMeshAgent.SetDestination(targetGO.transform.position);
        }

        public void Stop() {
            Helpers.Nav.TogglePathing(_navMeshAgent, false);
        }

    }
}