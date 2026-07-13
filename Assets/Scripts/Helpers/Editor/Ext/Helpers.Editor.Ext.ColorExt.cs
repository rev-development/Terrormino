using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.Ext
{
	public static class ColorExtensions
	{
		public static StyleColor ToStyleColor(this Color color) => new(color);
	}
}