using System.Collections.Generic;
using Helpers;
using Helpers.Attributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EC.Tetris
{
	[CreateAssetMenu(fileName = "TetrisConfig", menuName = "Terrormino/Tetris/Config")]
	public class ConfigSO : InjectableSO<ConfigSO, ConfigData, IConfig>, IConfig
	{
		[field: SerializeField] public Vector2Int PlayfieldSize { get; set; } = new(10, 20);

		[FeatureNotImplemented] [field: SerializeField] public bool HardDropEnabled { get; set; } = true;

		[field: SerializeField] public bool GhostEnabled { get; set; } = true;

		[field: SerializeField] public float LockDelay { get; set; } = 0.5f;

		[field: SerializeField]
		public float DASDelay { get; set; } = 0.167f; // Tetris Guideline is 167ms for DAS, Original Tetris was 267ms

		[field: SerializeField] public float AutoRepeatRate { get; set; } = 0.033f; // Tetris Guideline is 33ms for ARR

		[field: SerializeField] public float SoftDropRate { get; set; } = 0.050f;

		[field: SerializeField] public int LockResetLimit { get; set; } = 15;

		[field: SerializeField] public List<Shape> Shapes { get; set; } = new();

		[field: SerializeField] public TileBase GhostTile { get; set; } = null;

		[field: SerializeField] public TileBase BgTile { get; set; } = null;
	}
}