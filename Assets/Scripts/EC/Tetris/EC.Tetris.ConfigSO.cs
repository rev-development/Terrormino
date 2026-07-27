using Helpers;
using UnityEngine;

namespace EC.Tetris
{
	[CreateAssetMenu(fileName = "TetrisConfig", menuName = "Terrormino/Tetris/Config")]
	public class ConfigSO : InjectableSO<ConfigSO, Config, IConfig>, IConfig

	{
		[field: SerializeField] public float LockDelay { get; set; }

		[field: SerializeField] public float MoveDelay { get; set; }

		[field: SerializeField] public bool HardDropEnabled { get; set; }

		[field: SerializeField] public bool GhostEnabled { get; set; }

		[field: SerializeField] public int BoardWidth { get; set; }

		[field: SerializeField] public int BoardHeight { get; set; }

		[field: SerializeField] public Vector2Int SpawnPosition { get; set; }
	}
}