using UnityEngine;
using UnityEngine.AI;
using Helpers;
using Helpers.Ext;

namespace Demon
{
	public class Pathing : MonoBehaviour
	{
		public float SlowZoneSpeed = 1.0f;

		//public float SlowZoneBrakeMultiplier = 4;
		public float SlowZoneAcceleration = 0.075f;

		private NavMeshAgent _agent;

		private Vector3 _destination;

		private bool _enteredSlow = false;

		private GameObject _player;

		public void Start()
		{
			_player = TryFind.ByTag("Player")?.GetComponentInChildren<Camera>().gameObject;
			_agent = gameObject.TryFindComponent<NavMeshAgent>();

			if (_player && _agent)
			{
				_destination = _player.transform.position;
				_agent.destination = _destination;
			}
		}

		public void Update()
		{
			if (NavMesh.SamplePosition(
					gameObject.transform.position,
					out var hit,
					5f,
					NavMesh.AllAreas
				)
				&& IntFromMask(hit.mask) == NavMesh.GetAreaFromName("Slow")
				&& !_enteredSlow)
			{
				//_agent.velocity = (_agent.velocity / SlowZoneBrakeMultiplier);
				_agent.velocity = Vector3.zero;
				_agent.acceleration = SlowZoneAcceleration;
				_agent.speed = SlowZoneSpeed;
				_enteredSlow = true;
			}
		}

		public static int IntFromMask(int mask)
		{
			for (var i = 0; i < 32; ++i)
				if (1 << i == mask)
					return i;

			return -1;
		}
	}
}