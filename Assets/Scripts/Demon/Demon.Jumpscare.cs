using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Demon
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Jumpscare : MonoBehaviour
    {

        public Vector3 JumpscareDemonPosition = new(0, -3.5f, 4.5f);

        public AudioSource Scream;

        public UnityEvent JumpscareStarted = new();

        public UnityEvent JumpscareEnded = new();

        private Demon.Manager.Manager _demonManager;

        private Player.Manager _playerManager;

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Manager playerManager))
            {
                playerManager.GameOver.Invoke();
            }
        }

        public IEnumerator JumpscareRoutine()
        {
            
            JumpscareStarted.Invoke();

            Scream.PlayOneShot(Scream.clip);

            yield return new WaitForSeconds(Scream.clip.length);

            JumpscareEnded.Invoke();
        }

    }
}