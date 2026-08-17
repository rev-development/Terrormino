using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EC.Tetris
{
	/// <summary>
	///     Concrete, in-memory IConfig — the struct default EventBus falls back to
	///     before any ScriptableObject-backed config (e.g. NightConfig) is applied
	///     via ApplyConfig.
	/// </summary>
	[Serializable]
	public class ConfigData : IConfig
	{
		public Vector2Int PlayfieldSize { get; set; }

		public bool HardDropEnabled { get; set; }

		public bool GhostEnabled { get; set; }

		public float LockDelay { get; set; }

		public float DASDelay { get; set; }

		public float AutoRepeatRate { get; set; }

		public float SoftDropRate { get; set; }

		public int LockResetLimit { get; set; }

		public List<Shape> Shapes { get; set; }

		public TileBase GhostTile { get; set; }

		public TileBase BgTile { get; set; }
	}
}