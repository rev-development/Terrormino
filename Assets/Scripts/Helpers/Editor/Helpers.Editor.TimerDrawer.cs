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
			var root = SolRoot();

			var elapsedTimeFloatField = SolFloatField(property.FindPropertyRelative("_elapsedTime"), true);

			var currentAlarmTimeFloatField = SolFloatField(property.FindPropertyRelative("_alarmTime"), true);

			var initializedBoolLabel = SolBooleanLabel(property.FindPropertyRelative("_initialized"));
			var dirtyBoolLabel = SolBooleanLabel(property.FindPropertyRelative("Dirty"));
			var runningBoolLabel = SolBooleanLabel(property.FindPropertyRelative("_running"));
			var ringingBoolLabel = SolBooleanLabel(property.FindPropertyRelative("_ringing"));

			var solGridItems = new[]
							   {
								   new VisualElement[]
								   {
									   elapsedTimeFloatField,
									   currentAlarmTimeFloatField,
								   },
								   new VisualElement[]
								   {
									   initializedBoolLabel,
									   dirtyBoolLabel,
									   runningBoolLabel,
									   ringingBoolLabel,
								   },
							   };

			var solGrid = SolGrid(solGridItems, property.displayName);
			root.Add(solGrid);

			root.Bind(property.serializedObject);

			return root;
		}
	}
}