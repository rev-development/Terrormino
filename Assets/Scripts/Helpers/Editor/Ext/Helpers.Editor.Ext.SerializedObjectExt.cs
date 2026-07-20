using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Helpers.Editor.Ext
{
	public static class SerializedObjectExt
	{
		public static void IterateProps(
			this SerializedObject so,
			VisualElement ele,
			Func<SerializedProperty, bool> skip = null,
			string[] classNames = null,
			Action<IStyle>[] styleActions = null
		)
		{
			var prop = so.GetIterator();

			if (prop.NextVisible(true))
				do
				{
					if (skip?.Invoke(prop) == true) continue;

					ele.Add(
						new PropertyField(prop)
							{
								name = prop.name,
							}.WithClass(classNames)
							 .WithStyle(styleActions)
					);
				} while (prop.NextVisible(false));

			ele.Bind(so);
		}
	}
}