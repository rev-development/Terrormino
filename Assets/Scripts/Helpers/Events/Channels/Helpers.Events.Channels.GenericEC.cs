using UnityEngine;
using UnityEngine.Events;

namespace Helpers.Events.Channels
{
    public abstract class GenericEC<T> : ScriptableObject
    {

        [Tooltip("The action to perform; Listeners subscribe to this UnityAction")]
        public UnityAction<T> OnEventRaised;

        public void RaiseEvent(T parameter)
        {
            OnEventRaised?.Invoke(parameter);
        }

    }
}