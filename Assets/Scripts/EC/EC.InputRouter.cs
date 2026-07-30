using System;
using System.Collections.Generic;
using Helpers.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace EC
{
	[AiGenerated("Claude", "Sonnet 4.6")]
	[AddComponentMenu("EC.InputRouter")]
	public class InputRouter : MonoBehaviour
	{
		[SerializeField] private List<Route> _routes = new();

		private readonly Dictionary<IXRSelectInteractor, InputActionMap> _grabbedActionMaps = new();

		private List<(InputAction Action, List<UnityEvent<InputAction>> Outputs)> _routedActions;
		private bool _cacheDirty = true;

		private void RebuildCache()
		{
			_routedActions = new List<(InputAction, List<UnityEvent<InputAction>>)>();

			foreach (var route in _routes)
			{
				InputAction match = null;

				foreach (var actionMap in _grabbedActionMaps.Values)
				{
					match = actionMap.FindAction(route.ActionRef.action.id);
					if (match != null) break;
				}

				if (match == null) continue;

				var existing = _routedActions.FindIndex(r => r.Action == match);
				if (existing >= 0)
					_routedActions[existing].Outputs.AddRange(route.Outputs);
				else
					_routedActions.Add((match, new List<UnityEvent<InputAction>>(route.Outputs)));
			}

			_cacheDirty = false;
		}

		private void Update()
		{
			if (_cacheDirty) RebuildCache();

			_routedActions.ForEach(entry =>
				{
					if (entry.Action.WasPerformedThisFrame() || entry.Action.WasReleasedThisFrame())
						entry.Outputs.ForEach(output => output.Invoke(entry.Action));
				}
			);
		}

		public void OnSelectEnter(SelectEnterEventArgs context)
		{
			if (context.interactorObject.transform.gameObject.TryGetComponent(out ActionBasedController controller))
			{
				_grabbedActionMaps[context.interactorObject] = controller.selectAction.action.actionMap;
				_cacheDirty = true;
			}
		}

		public void OnSelectExit(SelectExitEventArgs context)
		{
			if (_grabbedActionMaps.Remove(context.interactorObject))
				_cacheDirty = true;
		}

#if UNITY_EDITOR
		private void OnValidate() => _cacheDirty = true;
#endif

		[Serializable]
		public class Route
		{
			public InputActionReference ActionRef;

			public List<UnityEvent<InputAction>> Outputs = new();
		}
	}
}
