using System.Collections.Generic;
using Helpers.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace Helpers.Ext
{
	/// <summary>
	///     Extension methods for <see cref="GameObject" />.
	/// </summary>
	[PublicAPI]
	public static class GameObjectExt
	{
		/// <summary>
		///     Wrapper for <see cref="GameObject.GetComponent{T}" /> that logs a warning to the Console
		///     when the component is missing, instead of failing silently.
		/// </summary>
		/// <remarks>
		///     The warning is logged with <paramref name="gameObject" /> as its context object,
		///     so clicking the Console entry pings the offending GameObject in the Hierarchy.
		/// </remarks>
		/// <param name="gameObject">The GameObject to search for the component.</param>
		/// <typeparam name="T">The type of component to find. Must derive from <see cref="Component" />.</typeparam>
		/// <returns>
		///     The attached component of type <typeparamref name="T" />,
		///     or <see langword="null" /> if none is attached.
		/// </returns>
		[CanBeNull]
		public static T TryFindComponent<T>(this GameObject gameObject)
			where T : Component
		{
			// This is function uses a generic type parameter, named T
			// These will look scary at first, but it is literally just a way to pass a class as a parameter
			// "where T : className" adds a constraint that whatever T is, it must inherit a certain class
			var foundComponent = gameObject.GetComponent<T>();

#if UNITY_EDITOR
			if (foundComponent == null)
				Debug.LogWarning($"Could not find {typeof(T).Name} on {gameObject.name}", gameObject);
#endif

			// If the component wasn't actually found, then this will return null because pretty much everything in Unity is a nullable type
			return foundComponent;
		}

		/// <summary>
		///     Finds a component of type <typeparamref name="T" /> on <paramref name="gameObject" /> and returns
		///     the value of the named property on that component.
		///     Logs a warning if the component or property is not found.
		/// </summary>
		/// <param name="gameObject">The GameObject to search for the component.</param>
		/// <param name="propertyName">The name of the property to read via reflection.</param>
		/// <typeparam name="T">The type of component to find. Must derive from <see cref="Component" />.</typeparam>
		/// <typeparam name="TComponent">The expected type of the property value.</typeparam>
		/// <returns>
		///     The property value cast to <typeparamref name="TComponent" />,
		///     or <see langword="default" /> if the component or property is not found.
		/// </returns>
		[AiGenerated("Claude", "Fable 5")]
		public static TComponent TryFindPropertyInComponent<T, TComponent>(
			this GameObject gameObject,
			string propertyName
		)
			where T : Component
		{
			var foundComponent = gameObject.TryFindComponent<T>();

			// Must bail out here: in a build a missing component is a real null (dereferencing throws NRE),
			// and in the Editor it is a fake-null placeholder whose members must not be accessed
			if (foundComponent == null) return default;

			var foundProperty = foundComponent.GetType().GetProperty(propertyName);

			if (foundProperty != null) return (TComponent)foundProperty.GetValue(foundComponent);

#if UNITY_EDITOR
			Debug.LogWarning($"Could not find a property named {propertyName} on {gameObject.name}", gameObject);
#endif

			// This makes it so the code relying on it never fails, but results in false positives
			return default;
		}

		/// <summary>
		///     Returns <paramref name="gameObject" /> if it is already assigned, otherwise searches for a
		///     GameObject by name. Useful for lazy-initializing Inspector-assignable references.
		/// </summary>
		/// <param name="gameObject">The existing reference to check.</param>
		/// <param name="name">The name to search by if <paramref name="gameObject" /> is <see langword="null" />.</param>
		/// <returns>
		///     <paramref name="gameObject" /> if already assigned;
		///     otherwise the first active GameObject named <paramref name="name" />,
		///     or <see langword="null" /> if none is found.
		/// </returns>
		public static GameObject TryFindGameObjectByNameOnlyIfNull(this GameObject gameObject, string name) =>
			// If the GameObject is null, try to find it
			// If the GameObject is already assigned, just hand it back
			gameObject == null ? TryFind.GameObjectByName(name) : gameObject;

		/// <summary>
		///     Logs a warning if <paramref name="toCheck" /> is <see langword="null" />, indicating that a
		///     required reference was not assigned in the Inspector. Logs with <paramref name="gameObject" />
		///     as context so clicking the Console entry pings the offending GameObject.
		/// </summary>
		/// <param name="gameObject">The GameObject that owns the field being checked.</param>
		/// <param name="toCheck">The value to check for <see langword="null" />.</param>
		/// <param name="name">The field name to include in the warning message.</param>
		public static void CheckIfSetInInspector(this GameObject gameObject, object toCheck, string name)
		{
#if UNITY_EDITOR
			if (toCheck == null) Debug.LogWarning($"{name} in {gameObject} not set in Inspector", gameObject);
#endif
		}

		/// <summary>
		///     Logs a warning if <paramref name="toCheck" /> is empty, indicating that a required list was not
		///     populated in the Inspector.
		/// </summary>
		/// <param name="toCheck">The list to check.</param>
		/// <param name="name">The field name to include in the warning message.</param>
		/// <typeparam name="T">The element type of the list.</typeparam>
		/// <returns><paramref name="toCheck" />, allowing use inline in an assignment.</returns>
		public static List<T> CheckIfEmptyListInInspector<T>(this GameObject gameObject, List<T> toCheck, string name)
		{
#if UNITY_EDITOR
			if (toCheck.Count == 0) Debug.LogWarning($"{name} in {gameObject} is empty in Inspector", gameObject);
#endif

			return toCheck;
		}

		/// <summary>
		///     Wrapper for <see cref="GameObject.GetComponentInChildren{T}()" /> that logs a warning to the
		///     Console when no matching component is found in the GameObject's children.
		/// </summary>
		/// <param name="gameObject">The GameObject whose children to search.</param>
		/// <typeparam name="T">The type of component to find. Must derive from <see cref="Component" />.</typeparam>
		/// <returns>
		///     The first component of type <typeparamref name="T" /> found in <paramref name="gameObject" />'s
		///     children, or <see langword="null" /> if none is found.
		/// </returns>
		[CanBeNull]
		public static T TryFindComponentInChildren<T>(this GameObject gameObject)
			where T : Component
		{
			var matchedComponent = gameObject.GetComponentInChildren<T>();

#if UNITY_EDITOR
			if (matchedComponent == null)
				Debug.LogWarning($"Could not find {typeof(T).Name} in Children of {gameObject.name}", gameObject);
#endif

			return matchedComponent;
		}

		/// <summary>
		///     Wrapper for <see cref="GameObject.GetComponentsInChildren{T}()" /> that logs a warning to the
		///     Console when no matching components are found in the GameObject's children.
		/// </summary>
		/// <param name="gameObject">The GameObject whose children to search.</param>
		/// <typeparam name="T">The type of component to find. Must derive from <see cref="Component" />.</typeparam>
		/// <returns>
		///     All components of type <typeparamref name="T" /> found in <paramref name="gameObject" />'s
		///     children. Returns an empty array if none are found.
		/// </returns>
		public static T[] TryFindComponentsInChildren<T>(this GameObject gameObject)
			where T : Component
		{
			var matchedComponents = gameObject.GetComponentsInChildren<T>();

#if UNITY_EDITOR
			if (matchedComponents.Length == 0)
				Debug.LogWarning($"Could not find {typeof(T).Name} in Children of {gameObject.name}", gameObject);
#endif

			return matchedComponents;
		}

		/// <summary>
		///     Wrapper for <see cref="GameObject.GetComponentInParent{T}()" /> that logs a warning to the
		///     Console when no matching component is found in the GameObject's parent chain.
		///     Also logs if <paramref name="gameObject" /> has no parent at all.
		/// </summary>
		/// <param name="gameObject">The GameObject whose parent chain to search.</param>
		/// <typeparam name="T">The type of component to find. Must derive from <see cref="Component" />.</typeparam>
		/// <returns>
		///     The first component of type <typeparamref name="T" /> found in <paramref name="gameObject" />'s
		///     parent chain, or <see langword="null" /> if none is found.
		/// </returns>
		[CanBeNull]
		public static T TryFindComponentInParent<T>(this GameObject gameObject)
			where T : Component
		{
#if UNITY_EDITOR
			if (!gameObject.transform.parent)
				Debug.Log(
					$"Could not search for {typeof(T).Name} in parents of {gameObject.name} because it has no parent"
				);
#endif

			var matchedComponent = gameObject.GetComponentInParent<T>();

#if UNITY_EDITOR
			if (matchedComponent == null)
				Debug.LogWarning($"Could not find {typeof(T).Name} in Parents of {gameObject.name}", gameObject);
#endif

			return matchedComponent;
		}

		public static Bounds TryGetColliderBounds(this GameObject gameObject) =>
			gameObject.TryFindComponentsInChildren<Collider>().GetAllBounds();
	}
}