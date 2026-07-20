using Helpers.Ext;
using JetBrains.Annotations;
using UnityEngine;

namespace Helpers
{
	/// <summary>
	///     Scene-wide search utilities. Unlike <see cref="Helpers.Ext.GameObjectExt" /> extension methods,
	///     these have no natural receiver — they search the entire active scene.
	/// </summary>
	[PublicAPI]
	public static class TryFind
	{
		/// <summary>
		///     Wrapper for <see cref="GameObject.Find" /> that logs a warning to the Console
		///     when no matching GameObject is found.
		/// </summary>
		/// <param name="name">The name of the GameObject to search for.</param>
		/// <returns>
		///     The first active GameObject named <paramref name="name" />,
		///     or <see langword="null" /> if none exists in the scene.
		/// </returns>
		[CanBeNull]
		public static GameObject GameObjectByName(string name)
		{
			var foundGameObject = GameObject.Find(name);

#if UNITY_EDITOR
			if (foundGameObject == null) UnityEngine.Debug.LogWarning($"Could not find GameObject {name}");
#endif

			return foundGameObject;
		}

		/// <summary>
		///     Finds a GameObject by name and returns a component of type <typeparamref name="T" /> attached to it.
		///     Logs a warning if either the GameObject or the component is not found.
		/// </summary>
		/// <param name="name">The name of the GameObject to search for.</param>
		/// <typeparam name="T">The type of component to find. Must derive from <see cref="Component" />.</typeparam>
		/// <returns>
		///     The component of type <typeparamref name="T" /> on the named GameObject,
		///     or <see langword="null" /> if the GameObject or component is not found.
		/// </returns>
		[CanBeNull]
		public static T ComponentOnGameObjectByName<T>(string name)
			where T : Component
		{
			var foundGameObject = GameObjectByName(name);

			return foundGameObject != null ? foundGameObject.TryFindComponent<T>() : null;
		}

		/// <summary>
		///     Wrapper for <see cref="GameObject.FindGameObjectWithTag" /> that logs a warning to the Console
		///     when no matching GameObject is found.
		/// </summary>
		/// <param name="tag">The tag to search for.</param>
		/// <returns>
		///     The first active GameObject tagged with <paramref name="tag" />,
		///     or <see langword="null" /> if none exists in the scene.
		/// </returns>
		[CanBeNull]
		public static GameObject ByTag(string tag)
		{
			var matchedObject = GameObject.FindGameObjectWithTag(tag);

#if UNITY_EDITOR
			if (matchedObject == null) UnityEngine.Debug.LogWarning($"Could not find GameObject with tag: {tag}");
#endif

			return matchedObject;
		}

		/// <summary>
		///     Finds a GameObject by tag and returns a component of type <typeparamref name="T" /> attached to it.
		///     Logs a warning if either the GameObject or the component is not found.
		/// </summary>
		/// <param name="tag">The tag to search for.</param>
		/// <typeparam name="T">The type of component to find. Must derive from <see cref="Component" />.</typeparam>
		/// <returns>
		///     The component of type <typeparamref name="T" /> on the tagged GameObject,
		///     or <see langword="null" /> if the GameObject or component is not found.
		/// </returns>
		[CanBeNull]
		public static T ComponentOnGameObjectByTag<T>(string tag)
			where T : Component
		{
			var matchedObject = ByTag(tag);

			// ByTag already logged the warning — bail out before dereferencing a null receiver
			return matchedObject != null ? matchedObject.TryFindComponent<T>() : null;
		}
	}
}
