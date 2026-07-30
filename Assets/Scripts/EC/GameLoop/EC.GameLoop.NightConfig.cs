using System;
using UnityEngine;

namespace EC.GameLoop
{
	[Serializable]
	public class NightConfig : ScriptableObject, EC.Tetris.IConfig
	{
		public string Label;

		public int LinesRequired;

		public string CutsceneName;

		public EC.Demon.ConfigSO DemonConfigSO;

		public EC.Tetris.ConfigSO TetrisConfig;

		[field: SerializeField] public float LockDelay { get; set; }

		[field: SerializeField] public float DASDelay { get; set; }

		[field: SerializeField] public bool HardDropEnabled { get; set; }

		[field: SerializeField] public bool GhostEnabled { get; set; }

		[field: SerializeField] public int BoardWidth { get; set; }

		[field: SerializeField] public int BoardHeight { get; set; }

		[field: SerializeField] public int LockResetLimit { get; set; }
	}
}