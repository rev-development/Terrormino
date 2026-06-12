using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Helpers.Events.Channels
{
    public abstract class GenericEC<T> : ScriptableObject
    {

        [Tooltip("When enabled, each time an event is raised, the parameters are added to a list")]
        public bool CollectParams = false;

        [System.NonSerialized] public List<T> CollectedParams = new();

        [Tooltip("The action to perform; Listeners subscribe to this UnityAction")]
        public UnityAction<T> OnEventRaised;

        public void RaiseEvent(T parameter) {
            OnEventRaised?.Invoke(parameter);

            if (CollectParams) CollectedParams.Add(parameter);
        }

    }
}