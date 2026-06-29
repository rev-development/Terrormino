using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Controls
{
    // Attach to the flashlight GameObject alongside its XRGrabInteractable and Rigidbody.
    // If the flashlight is dropped (not currently held), it teleports back in front
    // of the player and freezes there with physics off, until the player grabs it again.
    public class FlashlightRespawn : MonoBehaviour
    {
        [Tooltip("The player/XR rig transform used to calculate the respawn position.")]
        public Transform PlayerTransform;

        [Tooltip("How far in front of the player the flashlight respawns.")]
        public float RespawnDistance = 0.5f;

        [Tooltip("How far below eye level the flashlight respawns (negative = lower).")]
        public float RespawnHeightOffset = -0.3f;

        private Rigidbody _rigidbody;
        private XRGrabInteractable _grabInteractable;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _grabInteractable = GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            _grabInteractable.selectExited.AddListener(OnDropped);
        }

        private void OnDisable()
        {
            _grabInteractable.selectExited.RemoveListener(OnDropped);
        }

        private void OnDropped(SelectExitEventArgs args)
        {
            // If there's still another interactor holding it (e.g. two-handed swap),
            // don't respawn -- it's not actually fully dropped.
            if (_grabInteractable.interactorsSelecting.Count > 0) return;

            Respawn();
        }

        private void Respawn()
        {
            if (PlayerTransform == null) return;

            // Position: forward from the player, lowered slightly
            Vector3 forwardFlat = PlayerTransform.forward;
            forwardFlat.y = 0f;
            forwardFlat.Normalize();

            Vector3 respawnPosition = PlayerTransform.position
                + forwardFlat * RespawnDistance
                + Vector3.up * RespawnHeightOffset;

            transform.position = respawnPosition;
            transform.rotation = Quaternion.identity;

            // Freeze physics entirely -- stays exactly where placed until regrabbed
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        // Called automatically by XRGrabInteractable's selectEntered if wired in Inspector,
        // or call this manually from a listener on selectEntered.
        public void OnPickedUp()
        {
            // Hand physics control back now that the player is holding it again
            _rigidbody.isKinematic = false;
        }
    }
}