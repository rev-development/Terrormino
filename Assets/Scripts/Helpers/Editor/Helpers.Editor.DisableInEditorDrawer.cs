using UnityEditor;
using UnityEngine;

namespace Helpers.Editor
{
	[CustomPropertyDrawer(typeof(DisableInEditorAttribute))]
	public class DisableInEditorDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.text += " (Auto-Initialized)";
			GUI.enabled = false; // Grey out field

			EditorGUI.PropertyField(
				position,
				property,
				label,
				true
			);

			GUI.enabled = true; // Re-enable for other fields
		}
	}
}