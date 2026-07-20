using System.Runtime.CompilerServices;
using UnityEngine;

namespace Helpers
{
	// This class is set up so we can basically copy and paste it into every project we work on
	public static class Initialized
	{
		public static void Warn(bool initialized, string name, [CallerMemberName] string callerName = "")
		{
			if (!initialized) Debug.Log($"{name} called {callerName} before being Initialized");
		}

		public static void Warn(bool initialized, GameObject gameObject, [CallerMemberName] string callerName = "")
		{
			if (!initialized) Debug.Log($"{gameObject.name} called {callerName} before being Initialized", gameObject);
		}
	}
}