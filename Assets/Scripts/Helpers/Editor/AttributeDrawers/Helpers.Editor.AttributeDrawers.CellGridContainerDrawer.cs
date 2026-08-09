using System.Linq;
using Helpers.Attributes;
using Helpers.Editor.Theming.SolarizedDark;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.AttributeDrawers
{
	/// <summary>
	///     Draws a [CellGridContainer]-tagged field's children, replacing any Vector2Int[]
	///     child that carries [CellGrid] with a themed toggleable grid foldout.
	/// </summary>
	[AiGenerated("Claude", "Fable 5")]
	[CustomPropertyDrawer(typeof(CellGridContainerAttribute))]
	public class CellGridContainerDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			// Stylesheet rides on the root, so every Sol class below — including the
			// cell grid and the plain PropertyFields — resolves without re-applying.
			var root = Ele.SolFoldout(property.displayName);
			StyleHelper.ApplyTo(root);

			var child = property.Copy();
			var end = property.GetEndProperty();
			child.NextVisible(true);

			while (!SerializedProperty.EqualContents(child, end))
			{
				var attr = fieldInfo.FieldType.GetField(child.name)
													 ?.GetCustomAttributes(typeof(CellGridAttribute), false)
														.FirstOrDefault() as CellGridAttribute;

				root.Add(attr != null ? BuildCellGrid(child.Copy(), attr) : new PropertyField(child.Copy()));

				child.NextVisible(false);
			}

			return root;
		}

		private static VisualElement BuildCellGrid(SerializedProperty arrayProp, CellGridAttribute attr)
		{
			var foldout = Ele.SolFoldout(arrayProp.displayName);
			var valueList = Ele.SolCol();

			var grid = Ele.SolGrid();

			for (var row = 0; row < attr.Rows; row++)
			{
				Ele.AppendSolGrid(
					grid,
					Enumerable.Range(0, attr.Columns)
										.Select(col =>
											 {
												 var coord = new Vector2Int(col, attr.Rows - 1 - row);

												 // Clicks only write to the array; the view updates solely
												 // through TrackPropertyValue observing the commit.
												 var btn = Ele.SolButtonSquare(_ =>
													 {
														 if (ContainsCoord(arrayProp, coord))
															 RemoveCoord(arrayProp, coord);
														 else
															 AddCoord(arrayProp, coord);

														 arrayProp.serializedObject.ApplyModifiedProperties();
													 }
												 );

												 btn.userData = coord;

												 return (VisualElement)btn;
											 }
										 )
										.ToArray()
				);
			}

			// VCol defaults to flex-grow: 1; the button cells must not stretch,
			// or the grid spreads to fill the row instead of staying compact.
			grid.Query<Button>().ForEach(btn => btn.parent.style.flexGrow = 0);

			foldout.Add(
				Ele.SolGrid(
					new[]
					{
						grid,
						valueList,
					}
				)
			);

			Refresh();

			// TrackPropertyValue can't see into Generic/array properties, so observe
			// the whole object — still fires on undo/redo and external edits.
			foldout.TrackSerializedObjectValue(arrayProp.serializedObject, _ => Refresh());

			return foldout;

			void Refresh()
			{
				grid.Query<Button>()
						.ForEach(btn => btn.EnableInClassList(
								 StyleHelper.BgCyan,
								 ContainsCoord(arrayProp, (Vector2Int)btn.userData)
							 )
						 );

				valueList.Clear();

				for (var i = 0; i < arrayProp.arraySize; i++)
				{
					var coord = arrayProp.GetArrayElementAtIndex(i).vector2IntValue;
					valueList.Add(Ele.SolLabel($"({coord.x}, {coord.y})", secondary: true));
				}
			}
		}

		private static bool ContainsCoord(SerializedProperty arrayProp, Vector2Int coord)
		{
			for (var i = 0; i < arrayProp.arraySize; i++)
				if (arrayProp.GetArrayElementAtIndex(i).vector2IntValue == coord)
					return true;

			return false;
		}

		private static void AddCoord(SerializedProperty arrayProp, Vector2Int coord)
		{
			arrayProp.arraySize++;
			arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1).vector2IntValue = coord;
		}

		private static void RemoveCoord(SerializedProperty arrayProp, Vector2Int coord)
		{
			for (var i = 0; i < arrayProp.arraySize; i++)
			{
				if (arrayProp.GetArrayElementAtIndex(i).vector2IntValue != coord) continue;

				arrayProp.DeleteArrayElementAtIndex(i);

				return;
			}
		}
	}
}