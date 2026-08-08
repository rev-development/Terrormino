using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.Ext
{
	/// <summary>
	///     Overloads follow the CSS shorthand for four-sided properties.
	///     1 value: All
	///     2 values Vertical Horizontal
	///     3 values: Top Horizontal Bottom
	///     4 values: Top Right Left Bottom
	/// </summary>
	[PublicAPI]
	public static class StyleExt
	{
		public static void SetBorderRadius(this IStyle style, int value)
		{
			style.borderTopLeftRadius = value;
			style.borderTopRightRadius = value;
			style.borderBottomLeftRadius = value;
			style.borderBottomRightRadius = value;
		}

		public static void SetPadding(this IStyle style, int value) =>
			style.SetPadding(
				value,
				value,
				value,
				value
			);

		public static void SetPadding(this IStyle style, int topBottom, int rightLeft) =>
			style.SetPadding(
				topBottom,
				rightLeft,
				topBottom,
				rightLeft
			);

		public static void SetPadding(this IStyle style, int top, int rightLeft, int bottom) =>
			style.SetPadding(
				top,
				rightLeft,
				bottom,
				rightLeft
			);

		public static void SetPadding(this IStyle style, int top, int right, int bottom, int left)
		{
			style.paddingTop = top;
			style.paddingRight = right;
			style.paddingBottom = bottom;
			style.paddingLeft = left;
		}

		public static void SetMargin(this IStyle style, int value) =>
			style.SetMargin(
				value,
				value,
				value,
				value
			);

		public static void SetMargin(this IStyle style, int topBottom, int rightLeft) =>
			style.SetMargin(
				topBottom,
				rightLeft,
				topBottom,
				rightLeft
			);

		public static void SetMargin(this IStyle style, int top, int rightLeft, int bottom) =>
			style.SetMargin(
				top,
				rightLeft,
				bottom,
				rightLeft
			);

		public static void SetMargin(this IStyle style, int top, int right, int bottom, int left)
		{
			style.marginTop = top;
			style.marginRight = right;
			style.marginBottom = bottom;
			style.marginLeft = left;
		}

		public static void SetBorderWidth(this IStyle style, int width)
		{
			style.borderLeftWidth = width;
			style.borderTopWidth = width;
			style.borderRightWidth = width;
			style.borderBottomWidth = width;
		}

		public static void SetBorderColor(this IStyle style, StyleColor styleColor)
		{
			style.borderLeftColor = styleColor;
			style.borderTopColor = styleColor;
			style.borderRightColor = styleColor;
			style.borderBottomColor = styleColor;
		}

		public static void SetBorderColor(this IStyle style, Color color) => style.SetBorderColor(color.ToStyleColor());

		public static void SetBorderColor(this IStyle style, string color) => style.SetBorderColor(color.ToColor());

		public static void SetBorder(this IStyle style, int width, StyleColor styleColor, int radius = 0)
		{
			style.SetBorderWidth(width);
			style.SetBorderColor(styleColor);
			style.SetBorderRadius(radius);
		}

		public static void SetBorder(this IStyle style, int width, Color color, int radius = 0)
		{
			style.SetBorderWidth(width);
			style.SetBorderColor(color);
			style.SetBorderRadius(radius);
		}

		public static void SetBorder(this IStyle style, int width, string color, int radius = 0)
		{
			style.SetBorderWidth(width);
			style.SetBorderColor(color);
			style.SetBorderRadius(radius);
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