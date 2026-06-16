using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    [Serializable]
    public class NonPersistentListenerTracker
    {

        [SerializeField] public List<NonPersistentListenerDisplay> NonPersistentListeners = new();

        public void Add(Component component, string unityEvent, string unityAction) {
            NonPersistentListeners.Add(new NonPersistentListenerDisplay(component, unityEvent, unityAction));
        }

        public void Remove(Component component, string unityEvent, string unityAction) {
            var match = NonPersistentListeners.Find(nonPersistentListener =>
                    nonPersistentListener.Component == component
                    && nonPersistentListener.UnityEvent == unityEvent
                    && nonPersistentListener.UnityAction == unityAction
                );

            if (match != null) NonPersistentListeners.Remove(match);
        }

        [Serializable]
        public class NonPersistentListenerDisplay
        {

            [SerializeField] public Component Component;

            [SerializeField] public string UnityEvent;

            [SerializeField] public string UnityAction;

            public NonPersistentListenerDisplay(Component component, string unityEvent, string unityAction) {
                Component = component;
                UnityEvent = unityEvent;
                UnityAction = unityAction;
            }

        }

    }
}