using System;
using UnityEngine;

namespace EC.GameLoop
{
	[Serializable]
	public class NightConfig : ScriptableObject
	{
		public string Label;

		public int LinesRequired;

		public string CutsceneName;

		public EC.Demon.ConfigSO DemonConfigSO;

		public EC.Tetris.ConfigSO TetrisConfig;
	}
}