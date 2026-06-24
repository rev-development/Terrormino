using System;
using EC.Demon;
using Tetris;
using UnityEngine;

namespace EC.GameLoop
{
	[Serializable]
	public class NightConfig : ScriptableObject, ITetrisConfig
	{
		public string Label;

		public int LinesRequired;

		public string CutsceneName;

		public Config DemonConfig;

		[field: SerializeField] public float TetrisGravityDelay { get; set; }

		[field: SerializeField] public float TetrisMoveDelay { get; set; }

		[field: SerializeField] public float TetrisLockDelay { get; set; }
	}
}