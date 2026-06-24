using UnityEngine;

namespace Helpers
{
	public class NavBeacon : MonoBehaviour
	{
		public Helpers.Events.Channels.GameObjectEC NavBeaconEC;

		private void Awake()
		{
			NavBeaconEC.RaiseEvent(gameObject);
		}
	}
}