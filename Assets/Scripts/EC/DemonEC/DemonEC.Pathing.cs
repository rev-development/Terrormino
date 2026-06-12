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

        public List<GameObject> NavBeacons = new();

        public void Awake() {
            _navMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);

            AgentSteeringConfig.Apply(_navMeshAgent);

            _eventBus = Helpers.Debug.TryFindComponent<EventBus>(gameObject);
        }

        public void Start() {
            NavBeacons = NavBeaconEC.CollectedParams;
        }

        public void OnEnable() {
            _eventBus.Illuminated.AddListener(OnIlluminated);
            _eventBus.BanishTriggered.AddListener(OnBanishTriggered);
        }

        private void OnBanishTriggered(GameObject arg0) {
            Helpers.Nav.TogglePathing(_navMeshAgent, false);
        }

        private void OnIlluminated(bool isIlluminated) {
            Helpers.Nav.TogglePathing(_navMeshAgent, !isIlluminated);
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