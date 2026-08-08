using System;
using UnityEngine;

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

		public int LockResetLimit { get; set; }
	}
}