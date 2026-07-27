using System;
using Helpers.Attributes;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     Concrete, in-memory IConfig — the struct default EventBus falls back to
	///     before any ScriptableObject-backed config (e.g. NightConfig) is applied
	///     via ApplyConfig.
	/// </summary>
	[Serializable]
	[AiGenerated("Claude", "claude-sonnet-4-6")]
	public struct Config : IConfig
	{
		public float LockDelay { get; set; }

		public float MoveDelay { get; set; }

		public bool HardDropEnabled { get; set; }

		public bool GhostEnabled { get; set; }

		public int BoardWidth { get; set; }

		public int BoardHeight { get; set; }

		public Vector2Int SpawnPosition { get; set; }
	}
}