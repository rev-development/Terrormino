using Helpers.Attributes;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     Every tunable Tetris value (timing, board size, spawn point, feature flags)
	///     in one contract, so both the plain Config struct and ScriptableObject-backed
	///     assets like EC.GameLoop.NightConfig can supply it interchangeably via
	///     EventBus.ApplyConfig. Gravity is intentionally absent — it's computed from
	///     Level via Rules.GetGravityDelay, not stored as config.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-4-6")]
	public interface IConfig
	{
		float LockDelay { get; set; }

		float MoveDelay { get; set; }

		bool HardDropEnabled { get; set; }

		bool GhostEnabled { get; set; }

		int BoardWidth { get; set; }

		int BoardHeight { get; set; }

		Vector2Int SpawnPosition { get; set; }
	}
}