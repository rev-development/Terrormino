using UnityEditor;
using UnityEngine;

namespace Helpers.Editor
{
	[CustomPropertyDrawer(typeof(NavMeshAreaMaskAttribute))]
	public class NavMeshAreaMaskDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.HelpBox(
					position,
					$"{nameof(NavMeshAreaMaskAttribute)} requires an int field.",
					MessageType.Error
				);

				return;
			}

			var areaNames = GameObjectUtility.GetNavMeshAreaNames();
			var areaIndices = new int[areaNames.Length];
			var maskValue = 0;

			for (var i = 0; i < areaNames.Length; i++)
			{
				areaIndices[i] = UnityEngine.AI.NavMesh.GetAreaFromName(areaNames[i]);
				if ((property.intValue & (1 << areaIndices[i])) != 0) maskValue |= 1 << i;
			}

			EditorGUI.BeginChangeCheck();

			var newMaskValue = EditorGUI.MaskField(
				position,
				label,
				maskValue,
				areaNames
			);

			if (EditorGUI.EndChangeCheck())
			{
				var newIntValue = 0;

				for (var i = 0; i < areaNames.Length; i++)
				{
					if ((newMaskValue & (1 << i)) != 0) newIntValue |= 1 << areaIndices[i];
				}

				property.intValue = newIntValue;
			}
		}
	}
}