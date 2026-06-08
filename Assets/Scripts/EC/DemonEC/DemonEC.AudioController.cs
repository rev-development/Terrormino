using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {

        public AudioClip Scream;

        [Helpers.DisableInEditor] [SerializeField] private AudioSource _audioSource;

        [Helpers.DisableInEditor] [SerializeField] private Controller _demonController;

        public UnityEvent JumpscareAudioEnded = new();

        private void Awake()
        {
            _audioSource = Helpers.Debug.TryFindComponent<AudioSource>(gameObject);

            _demonController = Helpers.Debug.TryFindComponentInParent<Controller>(gameObject);
        }

        private void OnEnable()
        {
            _demonController.JumpscareTriggered.AddListener(OnJumpscareTriggered);
        }

        private void OnJumpscareTriggered()
        {
            StartCoroutine(JumpscareAudioRoutine());
        }

        private IEnumerator JumpscareAudioRoutine()
        {
            _audioSource.PlayOneShot(Scream);

            yield return new WaitForSeconds(Scream.length);

            JumpscareAudioEnded.Invoke();
        }

    }
}