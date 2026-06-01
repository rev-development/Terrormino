using UnityEngine;
using UnityEngine.AI;

namespace Demon
{
    public class Pathing : MonoBehaviour
    {
        private GameObject _player;
        private Vector3 _destination;
        private NavMeshAgent _agent;

        public float SlowZoneSpeed = 1.0f;

        //public float SlowZoneBrakeMultiplier = 4;
        public float SlowZoneAcceleration = 0.075f;

        public void Start()
        {
            _player = Helpers
                .Debug.TryFindByTag("Player")
                .GetComponentInChildren<Camera>()
                .gameObject;
            _agent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);
            if (_player && _agent)
            {
                _destination = _player.transform.position;
                _agent.destination = _destination;
            }
        }

        public static int IntFromMask(int mask)
        {
            for (int i = 0; i < 32; ++i)
            {
                if ((1 << i) == mask)
                    return i;
            }
            return -1;
        }

        private bool _enteredSlow = false;

        public void Update()
        {
            if (
                NavMesh.SamplePosition(
                    gameObject.transform.position,
                    out NavMeshHit hit,
                    5f,
                    NavMesh.AllAreas
                )
                && IntFromMask(hit.mask) == NavMesh.GetAreaFromName("Slow")
                && !_enteredSlow
            )
            {
                //_agent.velocity = (_agent.velocity / SlowZoneBrakeMultiplier);
                _agent.velocity = Vector3.zero;
                _agent.acceleration = SlowZoneAcceleration;
                _agent.speed = SlowZoneSpeed;
                _enteredSlow = true;
            }
        }
    }
}
