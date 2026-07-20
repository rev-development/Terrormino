using UnityEngine;

namespace Helpers.Editor.Ext
{
	public static class ValueTupleExt
	{
		public static Color ToColor(this (float, float, float, float) tuple) =>
			new(
				Mathf.Clamp01(tuple.Item1 / 255f),
				Mathf.Clamp01(tuple.Item2 / 255f),
				Mathf.Clamp01(tuple.Item3 / 255f),
				Mathf.Clamp01(tuple.Item4 / 255f)
			);

		public static Color ToColor(this (float, float, float) tuple) =>
			new(
				Mathf.Clamp01(tuple.Item1 / 255f),
				Mathf.Clamp01(tuple.Item2 / 255f),
				Mathf.Clamp01(tuple.Item3 / 255f),
				255f
			);

		public static Color ToColor(this (int, int, int, int) tuple) =>
			new(
				Mathf.Clamp01(tuple.Item1 / 255f),
				Mathf.Clamp01(tuple.Item2 / 255f),
				Mathf.Clamp01(tuple.Item3 / 255f),
				Mathf.Clamp01(tuple.Item4 / 255f)
			);

		public static Color ToColor(this (int, int, int) tuple) =>
			new(
				Mathf.Clamp01(tuple.Item1 / 255f),
				Mathf.Clamp01(tuple.Item2 / 255f),
				Mathf.Clamp01(tuple.Item3 / 255f),
				255f
			);
	}
}