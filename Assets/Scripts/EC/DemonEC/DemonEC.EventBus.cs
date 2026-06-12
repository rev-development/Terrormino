using UnityEngine;
using UnityEngine.Events;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    public class EventBus : MonoBehaviour
    {

        [SerializeField] private Helpers.Events.Channels.GameObjectEC _removeDemon;

        [SerializeField] private Helpers.Events.Channels.VoidEC _gameOver;

        public UnityEvent<GameObject> BanishTriggered = new();

        public UnityEvent<GameObject> JumpscareTriggered = new();

        public UnityEvent JumpscareFxCompleted = new();

        public UnityEvent BanishFxCompleted = new();

        public UnityEvent<bool> Illuminated = new();

        [Helpers.DisableInEditor] [SerializeField] private ControlPanel _controlPanel;

        public void Awake()
        {
            _controlPanel = Helpers.Debug.TryFindComponent<ControlPanel>(gameObject);
        }

        private void OnEnable()
        {
            BanishFxCompleted.AddListener(OnBanishFxCompleted);
            JumpscareFxCompleted.AddListener(OnJumpscareFxCompleted);

            if (_controlPanel)
            {
                _controlPanel.ListenerTracker.Add(this, nameof(BanishFxCompleted), nameof(OnBanishFxCompleted));

                _controlPanel.ListenerTracker.Add(this, nameof(JumpscareFxCompleted), nameof(OnJumpscareFxCompleted));
            }
        }

        private void OnDisable()
        {
            BanishTriggered.RemoveAllListeners();
            JumpscareTriggered.RemoveAllListeners();
            JumpscareFxCompleted.RemoveAllListeners();
            BanishFxCompleted.RemoveAllListeners();
            Illuminated.RemoveAllListeners();
        }

        private void OnJumpscareFxCompleted()
        {
            _gameOver.RaiseEvent();
        }

        private void OnBanishFxCompleted()
        {
            _removeDemon.RaiseEvent(gameObject);
        }

    }
}