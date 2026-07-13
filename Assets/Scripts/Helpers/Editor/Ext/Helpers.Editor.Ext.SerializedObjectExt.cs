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
			Func<SerializedProperty, bool> skip = null
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
						}
					);
				} while (prop.NextVisible(false));

			ele.Bind(so);
		}
	}
}