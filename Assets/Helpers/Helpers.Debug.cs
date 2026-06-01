using UnityEngine;

namespace Helpers
{
    // This class is setup so we can basically copy and paste it into every project we work on
    public static class Debug
    {
        // This is function uses a generic type parameter, named T
        // These will look scary at first, but it is literally just a way to pass a class as a parameter
        // "where T : className" adds a constraint that whatever T is, it must inherit a certain class
        public static T TryFindComponent<T>(GameObject sourceObject)
            where T : Component
        {
            T foundComponent = sourceObject.GetComponent<T>();
            if (foundComponent == null)
            {
                UnityEngine.Debug.Log(
                    $"Could not find {typeof(T).Name} on {sourceObject.name}",
                    sourceObject
                );
            }
            // If the component wasn't actually found, then this will return null because pretty much everything in Unity is a nullable type
            return foundComponent;
        }

        public static V TryFindPropertyInComponent<T, V>(
            GameObject sourceObject,
            string propertyName
        )
            where T : Component
        {
            T foundComponent = sourceObject.GetComponent<T>();
            if (foundComponent == null)
            {
                UnityEngine.Debug.Log(
                    $"Could not find {typeof(T).Name} on {sourceObject.name}",
                    sourceObject
                );
            }

            if (foundComponent.GetType().GetProperty(propertyName) == null)
            {
                UnityEngine.Debug.Log(
                    $"Could not find a property named {propertyName} on {sourceObject.name}",
                    sourceObject
                );
                // This makes it so the code relying on it never fails, but results in false positives
                return default;
            }
            else
            {
                return (V)
                    foundComponent.GetType().GetProperty(propertyName).GetValue(foundComponent);
            }
        }

        public static GameObject TryFindGameObjectByName(string name)
        {
            GameObject foundGameObject = GameObject.Find(name);
            if (foundGameObject == null)
            {
                UnityEngine.Debug.Log($"Could not find GameObject {name}");
            }
            return foundGameObject;
        }

        public static T TryFindComponentOnGameObjectByName<T>(string name)
            where T : Component
        {
            GameObject foundGameObject = TryFindGameObjectByName(name);
            if (foundGameObject != null)
            {
                return TryFindComponent<T>(foundGameObject);
            }
            else
            {
                return null;
            }
        }

        public static GameObject TryFindGameObjectByNameOnlyIfNull(
            GameObject gameObject,
            string name
        )
        {
            // If the GameObject is null, try to find it
            // If the GameObject is already assigned, just hand it back
            if (gameObject == null)
            {
                return TryFindGameObjectByName(name);
            }
            else
            {
                return gameObject;
            }
        }

        public static void CheckIfSetInInspector(GameObject gameObject, object toCheck, string name)
        {
            if (toCheck == null)
            {
                UnityEngine.Debug.Log($"{name} in {gameObject} not set in Inspector", gameObject);
            }
        }

        public static void CheckIfSetInInspector(object toCheck, string name)
        {
            if (toCheck == null)
            {
                UnityEngine.Debug.Log($"{name} not set in Inspector");
            }
        }

        public static T TryFindComponentInChildren<T>(GameObject sourceObject)
            where T : Component
        {
            T matchedComponent = sourceObject.GetComponentInChildren<T>();
            if (matchedComponent == null)
            {
                UnityEngine.Debug.Log(
                    $"Could not find {typeof(T).Name} in Children of {sourceObject.name}",
                    sourceObject
                );
            }
            return matchedComponent;
        }

        public static T[] TryFindComponentsInChildren<T>(GameObject sourceObject)
            where T : Component
        {
            T[] matchedComponents = sourceObject.GetComponentsInChildren<T>();
            if (matchedComponents.Length == 0)
            {
                UnityEngine.Debug.Log(
                    $"Could not find {typeof(T).Name} in Children of {sourceObject.name}",
                    sourceObject
                );
            }
            return matchedComponents;
        }

        public static GameObject TryFindByTag(string tag)
        {
            GameObject matchedObject = GameObject.FindGameObjectWithTag(tag);

            if (matchedObject == null)
            {
                UnityEngine.Debug.Log($"Could not find GameObject with tag: {tag}");
            }

            return matchedObject;
        }

        public static T TryFindComponentOnGameObjectByTag<T>(string tag)
            where T : Component
        {
            GameObject matchedObject = TryFindByTag(tag);
            T matchedComponent = TryFindComponent<T>(matchedObject);

            if (matchedComponent == null)
            {
                UnityEngine.Debug.Log($"Could not find {typeof(T).Name} on {matchedObject.name}");
            }

            return matchedComponent;
        }
    }
}
