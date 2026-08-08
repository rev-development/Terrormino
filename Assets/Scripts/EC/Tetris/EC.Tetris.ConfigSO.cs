using Helpers;
using UnityEngine;

namespace EC.Tetris
{
	[CreateAssetMenu(fileName = "TetrisConfig", menuName = "Terrormino/Tetris/Config")]
	public class ConfigSO : InjectableSO<ConfigSO, ConfigData, IConfig>, IConfig
	{
		[field: SerializeField] public Vector2Int PlayfieldSize { get; set; } = new(10, 20);

		[field: SerializeField] public bool HardDropEnabled { get; set; } = true;

		[field: SerializeField] public bool GhostEnabled { get; set; } = true;

		[field: SerializeField] public float LockDelay { get; set; } = 0.5f;

		[field: SerializeField] public float DASDelay { get; set; } = 0.1f;

		[field: SerializeField] public int LockResetLimit { get; set; } = 15;
	}
}