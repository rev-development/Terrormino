namespace EC.Tetris
{
	/// <summary>
	///     Every tunable Tetris value (timing, board size, feature flags) in one
	///     contract. Gravity and spawn position are intentionally absent — both are
	///     computed from board dimensions via Rules, not stored as config.
	/// </summary>
	public interface IConfig
	{
		int BoardWidth { get; set; }

		int BoardHeight { get; set; }

		bool HardDropEnabled { get; set; }

		bool GhostEnabled { get; set; }

		float LockDelay { get; set; }

		float DASDelay { get; set; }

		int LockResetLimit { get; set; }
	}
}