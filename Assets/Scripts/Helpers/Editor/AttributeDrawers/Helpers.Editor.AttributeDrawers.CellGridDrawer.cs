using Helpers.Attributes;
using Helpers.Editor.Theming.SolarizedDark;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Helpers.Editor.Theming.SolarizedDark.Ele;

namespace Helpers.Editor.AttributeDrawers
{
	/// <summary>
	///     Draws a [CellGridContainer]-tagged field's children, replacing any Vector2Int[]
	///     child that carries [CellGrid] with a themed toggleable grid foldout.
	/// </summary>
	[AiGenerated("Claude", "Fable 5")]
	[CustomPropertyDrawer(typeof(CellGridAttribute))]
	public class CellGridDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = SolRoot();

			if (fieldInfo.FieldType != typeof(Vector2Int[])
					|| attribute is not CellGridAttribute cellGridAttribute)
				return root;

			var foldout = SolFoldout(property.displayName);
			root.Add(foldout);

			var gridSize = new Vector2Int(cellGridAttribute.Rows - 1, cellGridAttribute.Columns - 1);

			var gridBtns = new VisualElement[gridSize.x][];

			for (var x = 0; x < gridSize.x; x++)
			{
				var row = new VisualElement[gridSize.y];

				for (var y = 0; y < gridSize.y; y++)
				{
					var coordinate = new Vector2Int(x, y);

					var btn = SolButtonSquare(_ =>
						{
							if (ContainsCoordinates(property, coordinate))
								RemoveCoordinate(property, coordinate);
							else
								AddCoordinate(property, coordinate);

							property.serializedObject.ApplyModifiedProperties();
						}
					);

					row[y] = btn;

					btn.userData = coordinate;
				}

				gridBtns[x] = row;
			}

			var grid = SolGrid(gridBtns);
			var valueList = SolList();

			foldout.Add(
				SolGrid(
					new[]
					{
						grid,
						valueList,
					}
				)
			);

			Refresh();

			foldout.TrackSerializedObjectValue(property.serializedObject, _ => Refresh());

			return root;

			void Refresh()
			{
				// Off state falls back to VBtn's own Base02 background.
				grid.Query<Button>()
						.ForEach(btn => btn.EnableInClassList(
								 StyleHelper.BgCyan,
								 ContainsCoordinates(property, (Vector2Int)btn.userData)
							 )
						 );

				valueList.Clear();

				for (var i = 0; i < property.arraySize; i++)
				{
					var coord = property.GetArrayElementAtIndex(i).vector2IntValue;
					valueList.Add(SolLabel($"({coord.x}, {coord.y})", secondary: true));
				}
			}
		}

		private static bool ContainsCoordinates(SerializedProperty property, Vector2Int lookupVal)
		{
			if (property is not { isArray: true, }) return false;

			for (var i = 0; i < property.arraySize; i++)
			{
				using var
					element = property
					 .GetArrayElementAtIndex(
							i
						); // The 'using' statement ensures all our temp objects are cleaned up properly as soon as it exits whatever scope.

				if (element.vector2IntValue == lookupVal) return true;
			}

			return false;
		}

		private static void AddCoordinate(SerializedProperty property, Vector2Int coordinate)
		{
			property.arraySize++; // Cheaty Unity arrays can grow after creation but you have to bump size manually
			property.GetArrayElementAtIndex(property.arraySize - 1).vector2IntValue = coordinate;
		}

		private static void RemoveCoordinate(SerializedProperty property, Vector2Int coordinate)
		{
			for (var i = 0; i < property.arraySize; i++)
			{
				if (property.GetArrayElementAtIndex(i).vector2IntValue != coordinate) continue;

				property.DeleteArrayElementAtIndex(i);

				return;
			}
		}
	}
}