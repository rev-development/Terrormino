using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Logger = EC.Tetris.Logger;

namespace Tests.Tetris
{
	public class FirstSpawnPosition : TestingBase
	{
		[UnityTest]
		public IEnumerator IsInBounds()
		{
			(var logCallback, var getCount) = CountLogCodeInstances(Logger.Code.IsInBounds);

			Application.logMessageReceived += logCallback;

			Controller.StartGame();

			yield return null;

			Assert.That(getCount(), Is.EqualTo(4));
		}

		[UnityTest]
		public IEnumerator IsNOTOccupied()
		{
			(var logCallback, var getCount) = CountLogCodeInstances(Logger.Code.IsNOTOccupied);

			Application.logMessageReceived += logCallback;

			Controller.StartGame();

			yield return null;

			Assert.That(getCount(), Is.EqualTo(4));
		}

		[UnityTest]
		public IEnumerator IsValid()
		{
			Controller.StartGame();

			yield return null;

			ExpectCode(Logger.Code.IsValidPosition);
		}

		[UnityTest]
		public IEnumerator IsNotGameOver()
		{
			List<string> eventLog = new();

			EventBus.GameOver.AddListener(() => eventLog.Add(nameof(Logger.Code.GameOver)));
			EventBus.Spawned.AddListener(_ => eventLog.Add(nameof(Logger.Code.PieceSpawned)));
			Controller.StartGame();

			yield return null;

			Assert.That(eventLog, Does.Not.Contain(nameof(Logger.Code.GameOver)));
		}

		[UnityTest]
		public IEnumerator GetFirstActivePiece()
		{
			Controller.StartGame();

			yield return null;

			Assert.That(Controller.ActivePiece, IsNotNull);
		}

		// [UnityTest]
		// public IEnumerator WasRenderTriggered()
		// {
		// 	Controller.StartGame();
		//
		// 	yield return null;
		//
		// 	ExpectCode(Logger.Code.RenderTriggered);
		// }

		[UnityTest]
		public IEnumerator CheckTiles()
		{
			yield return null;

			Controller.Config.Shapes.ForEach(shape => Assert.That(shape.Tile, IsNotNull));
		}
	}
}