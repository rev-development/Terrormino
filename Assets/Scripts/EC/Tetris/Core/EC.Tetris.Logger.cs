using System.Collections.Generic;
using UnityEngine;

namespace EC.Tetris
{
	public class Logger
	{
		public enum Code
		{
			// UnityEvent: 100-199
			PieceMoved = 101,

			PieceRotated = 102,

			HardDrop = 103,

			PieceSpawned = 104,

			PieceLocked = 105,

			LinesCleared = 106,

			GameStart = 107,

			GameOver = 108,

			// Tetris Rules: 200-299

			IsValidPosition = 200,

			IsNOTValidPosition = 201,

			IsInBounds = 202,

			IsNOTInBounds = 203,

			IsOccupied = 204,

			IsNOTOccupied = 205,

			// Playfield Rendering

			RenderTriggered = 301,
		}

		public List<string> EventLog = new();

		public static void LogCode(Code code, List<string> log)
		{
			LogCode(code);
			log.Add(code.ToString());
		}

		public static void LogCode(Code code)
		{
#if UNITY_EDITOR
			Debug.Log(code.ToString());
#endif
		}
	}
}