using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Helpers.Editor
{
	[DisallowMultipleComponent]
	public abstract class ControlPanelBase : MonoBehaviour
	{
		/// <summary>
		///     Checks if the GameObject is currently in a scene and not just selected in the File Explorer
		/// </summary>
		/// <returns></returns>
		protected bool IsOutsideOfAScene() => string.IsNullOrEmpty(gameObject.scene.name);

		protected abstract List<MonoBehaviour> GetComponents();

		public List<MonoBehaviour> InitializeComponents()
		{
			var components = new List<MonoBehaviour>();

			if (IsOutsideOfAScene()) return components;

			foreach (var component in GetComponents())
			{
				if (component == null) continue;

				var awakeMethod = component.GetType().GetMethod("Awake", BindingFlags.Public | BindingFlags.Instance);

				if (awakeMethod == null) continue;

				awakeMethod.Invoke(component, null);
				components.Add(component);
			}

			return components;
		}

		public virtual List<MonoBehaviour> GetInitializedComponents() => InitializeComponents();
	}
}