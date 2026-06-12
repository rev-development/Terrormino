using UnityEngine;

namespace Helpers
{
    public class NavBeacon : MonoBehaviour
    {

        public Events.Channels.GameObjectEC NavBeaconEC;

        private void Awake()
        {
            NavBeaconEC.RaiseEvent(gameObject);
        }

    }
}