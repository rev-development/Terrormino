using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.Ext
{
	public static class StyleExt
	{
		public static void SetAllBorderRadius(this IStyle style, int value)
		{
			style.borderTopLeftRadius = value;
			style.borderTopRightRadius = value;
			style.borderBottomLeftRadius = value;
			style.borderBottomRightRadius = value;
		}

		public static void SetAllPadding(this IStyle style, int value) =>
			style.SetAllPadding(
				value,
				value,
				value,
				value
			);

		public static void SetAllPadding(this IStyle style, int topBottom, int rightLeft) =>
			style.SetAllPadding(
				topBottom,
				rightLeft,
				topBottom,
				rightLeft
			);

		public static void SetAllPadding(this IStyle style, int top, int rightLeft, int bottom) =>
			style.SetAllPadding(
				top,
				rightLeft,
				bottom,
				rightLeft
			);

		public static void SetAllPadding(this IStyle style, int top, int right, int bottom, int left)
		{
			style.paddingTop = top;
			style.paddingRight = right;
			style.paddingBottom = bottom;
			style.paddingLeft = left;
		}

		public static void SetAllBorderWidth(this IStyle style, int width)
		{
			style.borderLeftWidth = width;
			style.borderTopWidth = width;
			style.borderRightWidth = width;
			style.borderBottomWidth = width;
		}

		public static void SetAllBorderColor(this IStyle style, StyleColor styleColor)
		{
			style.borderLeftColor = styleColor;
			style.borderTopColor = styleColor;
			style.borderRightColor = styleColor;
			style.borderBottomColor = styleColor;
		}

		public static void SetAllBorderColor(this IStyle style, Color color) =>
			style.SetAllBorderColor(color.ToStyleColor());

		public static void SetAllBorderColor(this IStyle style, string color) =>
			style.SetAllBorderColor(color.ToColor());

		public static void SetAllBorder(this IStyle style, int width, StyleColor styleColor, int radius = 0)
		{
			style.SetAllBorderWidth(width);
			style.SetAllBorderColor(styleColor);
			style.SetAllBorderRadius(radius);
		}

		public static void SetAllBorder(this IStyle style, int width, Color color, int radius = 0)
		{
			style.SetAllBorderWidth(width);
			style.SetAllBorderColor(color);
			style.SetAllBorderRadius(radius);
		}

		public static void SetAllBorder(this IStyle style, int width, string color, int radius = 0)
		{
			style.SetAllBorderWidth(width);
			style.SetAllBorderColor(color);
			style.SetAllBorderRadius(radius);
		}

		public static void MergeFrom(this IStyle target, IStyle source)
		{
			foreach (var prop in typeof(IStyle).GetProperties())
			{
				var value = prop.GetValue(source);
				var keywordProp = value?.GetType().GetProperty("keyword");

				if (keywordProp == null) continue;

				var keyword = (StyleKeyword)keywordProp.GetValue(value);

				if (keyword == StyleKeyword.Undefined) continue;

				prop.SetValue(target, value);
			}
		}
	}
}