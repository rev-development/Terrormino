using Helpers.Editor.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.Ext
{
	public static class StringExt
	{
		public static Color ToColor(this string str) =>
			ColorUtility.TryParseHtmlString(str, out var color) ? color : (255f, 0f, 255f).ToColor();

		public static StyleColor ToStyleColor(this string str) =>
			ColorUtility.TryParseHtmlString(str, out var color)
				? color.ToStyleColor()
				: (255f, 0f, 255f).ToColor().ToStyleColor();
	}
}