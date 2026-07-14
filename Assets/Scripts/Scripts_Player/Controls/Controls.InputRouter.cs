using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Controls
{
    public class InputRouter : MonoBehaviour
    {
        [Serializable]
        public class Route
        {
            public InputActionReference ActionRef;
            public List<UnityEvent<InputAction>> Outputs = new();
        }

        public List<Route> Routes = new();
        private Dictionary<Guid, List<UnityEvent<InputAction>>> _aggregatedOutputs
        {
            get
            {
                // This flattens the Routes property
                // Uses each action's unique id as key and has list of all outputs attached to that action
                Dictionary<Guid, List<UnityEvent<InputAction>>> aggregatedRoutes = new();

                Routes.ForEach(route =>
                {
                    // Typical add these values to the list if there is a key, otherwise add new key
                    if (aggregatedRoutes.ContainsKey(route.ActionRef.action.id))
                    {
                        aggregatedRoutes[route.ActionRef.action.id].AddRange(route.Outputs);
                    }
                    else
                    {
                        aggregatedRoutes.Add(route.ActionRef.action.id, route.Outputs);
                    }
                });

                return aggregatedRoutes;
            }
        }

        // When the GameObject is grabbed, the thing that grabbed it adds its ActionMap (based on the Select Action AKA Grab) to this list
        // This is because we only want to route inputs of actions of a grabbing controller
        private readonly List<InputActionMap> _grabbedActionMaps = new();

        public void OnSelectEnter(SelectEnterEventArgs context)
        {
            if (
                context.interactorObject.transform.gameObject.TryGetComponent(
                    out ActionBasedController controller
                )
            )
            {
                _grabbedActionMaps.Add(controller.selectAction.action.actionMap);
            }
        }

        public void OnSelectExit(SelectExitEventArgs context)
        {
            if (
                context.interactorObject.transform.gameObject.TryGetComponent(
                    out ActionBasedController controller
                )
            )
            {
                _grabbedActionMaps.Remove(controller.selectAction.action.actionMap);
            }
        }

        private List<InputAction> _enabledActions
        {
            // Generates a list of InputActions by cross referencing the ActionMaps added by grabbing object and the InputActions of added routes
            get
            {
                List<InputAction> enabledActions = new();

                _grabbedActionMaps.ForEach(actionMap =>
                {
                    foreach (var guid in _aggregatedOutputs.Keys)
                    {
                        var match = actionMap.FindAction(guid);
                        if (match != null)
                        {
                            enabledActions.Add(match);
                        }
                    }
                });

                return enabledActions;
            }
        }

        public void Update()
        {
            // Each frame, go through the list of enabled registed inputs and check if they were performed, if yes then pass to the events
            _enabledActions.ForEach(action =>
            {
                if (action.WasPerformedThisFrame())
                {
                    _aggregatedOutputs[action.id].ForEach(output => output.Invoke(action));
                }
            });
        }
    }
}
