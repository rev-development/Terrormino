using UnityEngine;
using UnityEngine.Events;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(AudioController))]
    [RequireComponent(typeof(Rigidbody))]
    public class Controller : MonoBehaviour
    {

        [Helpers.DisableInEditor] public AnimationController AnimationController;

        [Helpers.DisableInEditor] public Health Health;

        [Helpers.DisableInEditor] public AudioController AudioController;

        public Helpers.Events.Channels.GameObjectEC GlobalBanish;

        [SerializeField] private Vector3 _jumpscareDemonPosition = new(0, -0.5f, -4.5f);

        public UnityEvent JumpscareTriggered = new();

        [SerializeField] private Helpers.Events.Channels.VoidEC _gameOver;

        private readonly GameOverChecklist _gameOverChecklist = new();

        private GameObject _modelRoot;

        private void Awake()
        {
            AnimationController = Helpers.Debug.TryFindComponentInChildren<AnimationController>(gameObject);

            if (AnimationController)
            {
                _modelRoot = AnimationController.gameObject;
            }

            Health = Helpers.Debug.TryFindComponent<Health>(gameObject);

            AudioController = Helpers.Debug.TryFindComponent<AudioController>(gameObject);
        }

        private void OnEnable()
        {
            if (AnimationController)
            {
                AnimationController.BanishAnimationEnded.AddListener(OnBanishAnimationEnded);
                AnimationController.JumpscareAnimationEnded.AddListener(OnJumpscareAnimationEnded);
            }

            if (AudioController)
            {
                AudioController.JumpscareAudioEnded.AddListener(OnJumpscareAudioEnded);
            }
        }

        private void OnDisable()
        {
            JumpscareTriggered.RemoveAllListeners();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player.Manager>(out var playerManager))
            {
                gameObject.transform.position = playerManager.gameObject.transform.position;
                _modelRoot.transform.localPosition = _jumpscareDemonPosition;
                JumpscareTriggered.Invoke();
            }
        }

        private void OnJumpscareAnimationEnded()
        {
            _gameOverChecklist.AnimationEnded = true;
            TryGameOver();
        }

        private void OnJumpscareAudioEnded()
        {
            _gameOverChecklist.AudioEnded = true;
            TryGameOver();
        }

        private void OnBanishAnimationEnded()
        {
            GlobalBanish.RaiseEvent(gameObject);
        }

        private void TryGameOver()
        {
            if (_gameOverChecklist.Valid)
            {
                _gameOver.RaiseEvent();
            }
        }

        private class GameOverChecklist
        {

            public bool AnimationEnded = false;

            public bool AudioEnded = false;

            public bool Valid => AudioEnded && AnimationEnded;

        }

    }
}