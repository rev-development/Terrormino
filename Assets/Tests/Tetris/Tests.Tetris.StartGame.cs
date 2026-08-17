using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Logger = EC.Tetris.Logger;

namespace Tests.Tetris
{
	public class StartGame : TestingBase
	{
		[UnityTest]
		public IEnumerator LogCodeTest()
		{
			EventBus.GameStart.AddListener(() => Logger.LogCode(Logger.Code.GameStart));
			Controller.StartGame();

			yield return null;

			ExpectCode(Logger.Code.GameStart);
		}

		[UnityTest]
		public IEnumerator ControllerHasConfig()
		{
			yield return null;

			Assert.That(Controller.Config, IsNotNull);
		}

		[UnityTest]
		public IEnumerator FirstSpawn()
		{
			Controller.StartGame();

			yield return null;

			Assert.That(Controller.ActivePiece, IsNotNull);
		}
	}
}