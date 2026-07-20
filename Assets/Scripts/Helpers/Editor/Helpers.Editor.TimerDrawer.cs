using Helpers.Editor.Ext;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static Helpers.Editor.Theming.SolarizedDark.Ele;

namespace Helpers.Editor
{
	[CustomPropertyDrawer(typeof(Timer))]
	public class TimerDrawer : PropertyDrawer
	{
		/// <summary>
		///     <para>Override this method to make your own UI Toolkit based GUI for the property.</para>
		/// </summary>
		/// <param name="property">The SerializedProperty to make the custom GUI for.</param>
		/// <returns>
		///     <para>The element containing the custom GUI.</para>
		/// </returns>
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = SolRoot().WithClass(Helpers.Editor.Theming.SolarizedDark.StyleHelper.VBorder);

			var elapsedTimeFloatField = SolFloatField(property.FindPropertyRelative("_elapsedTime"), true);
			elapsedTimeFloatField.style.marginLeft = 0;

			var currentAlarmTimeFloatField = SolFloatField(property.FindPropertyRelative("_alarmTime"), true);

			var initializedBoolLabel = SolBooleanLabel(property.FindPropertyRelative("_initialized"));
			var dirtyBoolLabel = SolBooleanLabel(property.FindPropertyRelative("Dirty"));
			var runningBoolLabel = SolBooleanLabel(property.FindPropertyRelative("_running"));
			var ringingBoolLabel = SolBooleanLabel(property.FindPropertyRelative("_ringing"));

			var solGrid = SolGrid(
				property.displayName,
				new[]
				{
					elapsedTimeFloatField,
					currentAlarmTimeFloatField,
				},
				new[]
				{
					initializedBoolLabel,
					dirtyBoolLabel,
					runningBoolLabel,
					ringingBoolLabel,
				}
			);

			root.Add(solGrid);

			root.Bind(property.serializedObject);

			return root;
		}
	}
}