using System;
using System.Collections;
using EC.Tetris;
using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Logger = EC.Tetris.Logger;
using Object = UnityEngine.Object;

namespace Tests.Tetris
{
	[PublicAPI]
	public class TestingBase
	{
		protected EC.Tetris.Controller Controller;

		protected EventBus EventBus;

		protected PlayfieldRenderer PlayfieldRenderer;

		// ReSharper disable once InconsistentNaming
		protected NullConstraint IsNotNull => Is.Not.Null;

		// ReSharper disable once InconsistentNaming
		protected TrueConstraint IsTrue => Is.True;

		protected void ExpectCode(Logger.Code code) => LogAssert.Expect(code.ToString());

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			yield return SceneManager.LoadSceneAsync("EC.Tetris.Test");

			Controller = Object.FindFirstObjectByType<Controller>();
			EventBus = Object.FindFirstObjectByType<EventBus>();
			PlayfieldRenderer = Object.FindFirstObjectByType<PlayfieldRenderer>();

			if (Controller == null
					|| EventBus == null
					|| PlayfieldRenderer == null)
				Assert.Inconclusive("Tetris is not set up correctly");
		}

		public static (Application.LogCallback Callback, Func<int> GetCount) CountLogCodeInstances(Logger.Code code)
		{
			var counter = 0;

			return (LogCallback, () => counter);

			// Explicitly define the 3 parameters required by Application.LogCallback
			void LogCallback(string logString, string stackTrace, LogType logType)
			{
				if (logString == code.ToString()) counter++;
			}
		}

		// private static IEnumerator CheckExists<T>(T param)
		// {
		// 	yield return null;
		//
		// 	Assert.That(param, IsNotNull);
		// }
		//
		// [UnityTest] public IEnumerator ControllerExists() => CheckExists(Controller);
		//
		// [UnityTest] public IEnumerator EventBusExists() => CheckExists(EventBus);
		//
		// [UnityTest] public IEnumerator PlayfieldRendererExists() => CheckExists(PlayfieldRenderer);
		// [UnityTearDown] public void TearDown() => SceneManager.UnloadSceneAsync("EC.Tetris.Test");
	}
}