using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Helpers.Events.Listeners
{
    public abstract class GenericECL<T> : MonoBehaviour
    {

        [Header("Listen to Event Channels")]
        [SerializeField] private Channels.GenericEC<T> _eventChannel;
        [Space] [Tooltip("Responds to receiving signal from Event Channel")] [SerializeField]
        private UnityEvent<T> _response;

        [SerializeField] private readonly float _delay = 0;

        private void OnEnable()
        {
            if (_eventChannel != null)
            {
                _eventChannel.OnEventRaised += OnEventRaised;
            }
        }

        private void OnDisable()
        {
            if (_eventChannel != null)
            {
                _eventChannel.OnEventRaised -= OnEventRaised;
            }
        }

        public void OnEventRaised(T value)
        {
            if (_delay is 0)
            {
                _response?.Invoke(value);
            }
            else
            {
                StartCoroutine(RaiseEventDelayed(value));
            }
        }

        private IEnumerator RaiseEventDelayed(T value)
        {
            yield return new WaitForSeconds(_delay);

            _response.Invoke(value);
        }

    }
}