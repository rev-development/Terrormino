using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Helpers
{
	[Serializable]
	public class UnityEventPlus<TEvent>
		where TEvent : UnityEventBase, new()
	{
		[SerializeField] private List<NonPersistentListener> _listeners = new();

		private TEvent _unityEvent = new();

		public virtual UnityEvent UnityEvent => _unityEvent;

		public List<NonPersistentListener> NonPersistentListeners => _listeners;

		public void AddListener(Component component, UnityAction unityAction)
		{
			_unityEvent.AddListener(unityAction);
			NonPersistentListeners.Add(new NonPersistentListener(component, unityAction));
		}

		public void RemoveListener(UnityAction unityAction)
		{
			var match = NonPersistentListeners.Find(listener => listener.UnityAction == unityAction);
			NonPersistentListeners.Remove(match);

			_unityEvent.RemoveListener(unityAction);
		}

		public void RemoveAllListeners() => _unityEvent.RemoveAllListeners();

		public void RemoveAllListenersAddedByThisComponent(Component component)
		{
			foreach (var nonPersistentListener in NonPersistentListeners.Where(listener => listener.Component == component))
				_unityEvent.RemoveListener(nonPersistentListener.UnityAction);
		}

		[Serializable]
		public class NonPersistentListener
		{
			[SerializeField] public Component Component;

			[SerializeField] public UnityAction UnityAction;

			public NonPersistentListener(Component component, UnityAction unityAction)
			{
				Component = component;
				UnityAction = unityAction;
			}
		}
	}

	[Serializable]
	public class UnityEventPlus<TEvent, T0> : UnityEventPlus<TEvent>
		where TEvent : UnityEvent<T0>, new()
	{
		public override UnityEvent _unityEvent { get; } = new T0();
	}
}