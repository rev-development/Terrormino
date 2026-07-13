using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Helpers.Events.Channels.GameObjectEC))]
	public class GameObjectECEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var root = new VisualElement();
			var gameObjectEC = (Helpers.Events.Channels.GameObjectEC)target;

			InspectorElement.FillDefaultInspector(root, new SerializedObject(gameObjectEC), this);

			root.Add(
				new Label
				{
					text = "CollectedParams (This field is Non-Serialized so its value is never saved)",
					style =
					{
						paddingLeft = 3,
					},
				}
			);

			var listView = new ListView
						   {
							   itemsSource = gameObjectEC.CollectedParams,
							   makeItem = () => new ObjectField(), // Create standard ObjectField for each entry
							   bindItem = (element, index) =>
							   {
								   var objectField = (ObjectField)element;
								   objectField.objectType = typeof(GameObject);
								   objectField.value = gameObjectEC.CollectedParams[index];
								   objectField.SetEnabled(false); // Make it read-only
							   },
							   showAddRemoveFooter = false, // Disable modifications
							   reorderable = false,
							   style =
							   {
								   backgroundColor
									   = Helpers.Editor.Theming.SolarizedDark.StyleHelper.ParseColor(
										   Helpers.Editor.Theming.SolarizedDark.Palette.Base00
									   ),
							   },
						   };

			listView.schedule.Execute(() =>
						 {
							 listView.itemsSource = gameObjectEC.CollectedParams;
							 listView.RefreshItems();
						 }
					 )
					.Every(100);

			root.Add(listView);

			return root;
		}
	}
}