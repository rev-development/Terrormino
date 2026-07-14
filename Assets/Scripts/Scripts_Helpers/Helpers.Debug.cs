using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    // This class is set up so we can basically copy and paste it into every project we work on
    public static class Debug
    {
        /// <summary>
        ///     Wrapper for TryFindComponent that returns the component or null with formatted error message
        /// </summary>
        /// <param name="sourceObject">The GameObject you are calling this from</param>
        /// <typeparam name="T">The component type you are looking for</typeparam>
        /// <returns>The component or null</returns>
        public static T TryFindComponent<T>(GameObject sourceObject) where T : Component
        {
            // This is function uses a generic type parameter, named T
            // These will look scary at first, but it is literally just a way to pass a class as a parameter
            // "where T : className" adds a constraint that whatever T is, it must inherit a certain class
            var foundComponent = sourceObject.GetComponent<T>();
            if (foundComponent == null)
                UnityEngine.Debug.LogWarning($"Could not find {typeof(T).Name} on {sourceObject.name}", sourceObject);
            // If the component wasn't actually found, then this will return null because pretty much everything in Unity is a nullable type
            return foundComponent;
        }

        public static TV TryFindPropertyInComponent<T, TV>(GameObject sourceObject, string propertyName)
            where T : Component
        {
            var foundComponent = sourceObject.GetComponent<T>();
            if (foundComponent == null)
                UnityEngine.Debug.LogWarning($"Could not find {typeof(T).Name} on {sourceObject.name}", sourceObject);

            if (foundComponent.GetType().GetProperty(propertyName) != null)
                return (TV)foundComponent.GetType().GetProperty(propertyName)!.GetValue(foundComponent);
            UnityEngine.Debug.LogWarning($"Could not find a property named {propertyName} on {sourceObject.name}",
                sourceObject);
            // This makes it so the code relying on it never fails, but results in false positives
            return default;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static GameObject TryFindGameObjectByName(string name)
        {
            var foundGameObject = GameObject.Find(name);
            if (foundGameObject == null) UnityEngine.Debug.LogWarning($"Could not find GameObject {name}");
            return foundGameObject;
        }

        public static T TryFindComponentOnGameObjectByName<T>(string name) where T : Component
        {
            var foundGameObject = TryFindGameObjectByName(name);
            return foundGameObject != null ? TryFindComponent<T>(foundGameObject) : null;
        }

        public static GameObject TryFindGameObjectByNameOnlyIfNull(GameObject gameObject, string name)
        {
            // If the GameObject is null, try to find it
            // If the GameObject is already assigned, just hand it back
            return gameObject == null ? TryFindGameObjectByName(name) : gameObject;
        }

        public static void CheckIfSetInInspector(GameObject gameObject, object toCheck, string name)
        {
            if (toCheck == null)
                UnityEngine.Debug.LogWarning($"{name} in {gameObject} not set in Inspector", gameObject);
        }

        public static void CheckIfSetInInspector(object toCheck, string name)
        {
            if (toCheck == null) UnityEngine.Debug.LogWarning($"{name} not set in Inspector");
        }

        public static List<T> CheckIfEmptyListInInspector<T>(List<T> toCheck, string name)
        {
            if (toCheck.Count == 0) UnityEngine.Debug.LogWarning($"{name} is empty in Inspector");
            return toCheck;
        }

        public static T TryFindComponentInChildren<T>(GameObject sourceObject) where T : Component
        {
            var matchedComponent = sourceObject.GetComponentInChildren<T>();
            if (matchedComponent == null)
                UnityEngine.Debug.LogWarning($"Could not find {typeof(T).Name} in Children of {sourceObject.name}",
                    sourceObject);
            return matchedComponent;
        }

        public static T[] TryFindComponentsInChildren<T>(GameObject sourceObject) where T : Component
        {
            var matchedComponents = sourceObject.GetComponentsInChildren<T>();
            if (matchedComponents.Length == 0)
                UnityEngine.Debug.LogWarning($"Could not find {typeof(T).Name} in Children of {sourceObject.name}",
                    sourceObject);
            return matchedComponents;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static GameObject TryFindByTag(string tag)
        {
            var matchedObject = GameObject.FindGameObjectWithTag(tag);

            if (matchedObject == null) UnityEngine.Debug.LogWarning($"Could not find GameObject with tag: {tag}");

            return matchedObject;
        }

        public static T TryFindComponentOnGameObjectByTag<T>(string tag) where T : Component
        {
            var matchedObject = TryFindByTag(tag);
            var matchedComponent = TryFindComponent<T>(matchedObject);

            if (matchedComponent == null)
                UnityEngine.Debug.LogWarning($"Could not find {typeof(T).Name} on {matchedObject.name}");

            return matchedComponent;
        }

        public static T TryFindComponentInParent<T>(GameObject sourceObject) where T : Component
        {
            var matchedComponent = sourceObject.GetComponentInParent<T>();
            if (matchedComponent == null)
                UnityEngine.Debug.LogWarning($"Could not find {typeof(T).Name} in Parents of {sourceObject.name}",
                    sourceObject);
            return matchedComponent;
        }
    }
}