using System;
using System.Collections;
using Helpers.Attributes;
using Helpers.Editor.Ext;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Helpers.Editor.Theming.SolarizedDark
{
	[PublicAPI]
	[AiGenerated("Claude", "Sonnet 4.6")]
	public static class Ele
	{
#region Root

		// Root container — call this instead of manually applying the stylesheet
		public static VisualElement SolRoot()
		{
			var root = new VisualElement();
			StyleHelper.ApplyTo(root);

			root.WithClass(
				StyleHelper.BgBase03,
				StyleHelper.FlexGrow,
				StyleHelper.VContainer,
				StyleHelper.BorderRadiusRounded
			);

			root.style.marginTop = 4;

			return root;
		}

#endregion

#region Layout

		public static VisualElement SolCard()
		{
			var card = new VisualElement();
			card.AddToClassList(StyleHelper.VCard);

			return card;
		}

		public static VisualElement SolCol(bool highlighted = false)
		{
			var col = new VisualElement();
			col.AddToClassList(StyleHelper.VCol);
			col.AddToClassList(highlighted ? StyleHelper.BgBase02 : StyleHelper.BgBase03);

			return col;
		}

		public static VisualElement SolContainer()
		{
			var container = new VisualElement();
			container.AddToClassList(StyleHelper.VContainer);
			container.AddToClassList(StyleHelper.BgBase03);

			return container;
		}

		public static VisualElement SolDivider()
		{
			var divider = new VisualElement();
			divider.AddToClassList(StyleHelper.VDivider);

			return divider;
		}

		public static Foldout SolFoldout(string text = null, bool value = false)
		{
			var foldout = new Foldout
			{
				text = text,
				value = value,
			};

			foldout.AddToClassList(StyleHelper.VField);
			foldout.AddToClassList(StyleHelper.VFoldout);

			return foldout;
		}

		public static ListView SolList(
			IList itemsSource = null,
			float itemHeight = 20f,
			Func<VisualElement> makeItem = null,
			Action<VisualElement, int> bindItem = null
		)
		{
			var list = new ListView(
				itemsSource,
				itemHeight,
				makeItem,
				bindItem
			);

			list.AddToClassList(StyleHelper.VField);
			list.AddToClassList(StyleHelper.VList);

			return list;
		}

		public static VisualElement SolPaper()
		{
			var paper = new VisualElement();
			paper.AddToClassList(StyleHelper.BgBase03);

			return paper;
		}

		public static VisualElement SolRow(bool highlighted = false)
		{
			var row = new VisualElement();
			row.AddToClassList(StyleHelper.VRow);
			row.AddToClassList(highlighted ? StyleHelper.BgBase02 : StyleHelper.BgBase03);

			return row;
		}

#endregion

#region Grid

		public static VisualElement SolGrid([ItemCanBeNull] params VisualElement[][] rows) => SolGrid(null, rows);

		public static VisualElement SolGrid(string label, [ItemCanBeNull] params VisualElement[][] rows)
		{
			var container = SolContainer();

			if (label != null) container.Add(SolLabel(label));

			AppendSolGrid(container, rows);

			return container;
		}

		public static VisualElement AppendSolGrid(VisualElement solGrid, [ItemCanBeNull] params VisualElement[][] rows)
		{
			foreach (var gridRow in rows)
			{
				var row = SolRow();

				if (gridRow != null)
					foreach (var gridItem in gridRow)
					{
						var col = SolCol();

						if (gridItem != null) col.Add(gridItem);

						row.Add(col);
					}

				solGrid.Add(row);
			}

			return solGrid;
		}

#endregion

#region Labels

		public static Label SolLabel() => SolLabel("");

		public static Label SolLabel(string text, bool emphasized = false, bool secondary = false)
		{
			var label = new Label(text);
			label.AddToClassList(StyleHelper.TextDefault);

			if (emphasized)
			{
				label.AddToClassList(StyleHelper.FontBold);
				label.AddToClassList(StyleHelper.TextBase2);
			}
			else if (secondary)
			{
				label.AddToClassList(StyleHelper.TextBase01);
				label.AddToClassList(StyleHelper.TextSm);
			}

			return label;
		}

#endregion

#region Buttons

		public static Button SolButton(string text = "", bool enabled = true)
		{
			var button = new Button
			{
				text = text,
			};

			button.AddToClassList(StyleHelper.VBtn);
			button.SetEnabled(enabled);

			return button;
		}

		public static Button SolButton(EventCallback<ClickEvent> onClick, string text = "", bool enabled = true)
		{
			var button = new Button
			{
				text = text,
			};

			button.RegisterCallback(onClick);
			button.AddToClassList(StyleHelper.VBtn);
			button.SetEnabled(enabled);

			return button;
		}

		public static Button SolButtonSquare(EventCallback<ClickEvent> onClick, bool enabled = true)
		{
			var button = new Button();

			button.RegisterCallback(onClick);
			button.AddToClassList(StyleHelper.VBtn);
			button.SetEnabled(enabled);
			button.style.width = 20f;
			button.style.height = 20f;

			return button;
		}

		public static Button SolButtonSquare(bool enabled = true)
		{
			var button = new Button();

			button.AddToClassList(StyleHelper.VBtn);
			button.SetEnabled(enabled);
			button.style.width = 20f;
			button.style.height = 20f;

			return button;
		}


#endregion

#region Fields

		public static VisualElement SolBooleanField(
			SerializedProperty prop,
			string trueValue = "True",
			string falseValue = "False",
			string label = null
		)
		{
			var row = SolRow()
							 .WithStyle(r =>
									{
										r.marginLeft = 0;
										r.marginRight = 0;
									}
								)
							 .WithClass(StyleHelper.VBorder, StyleHelper.VBooleanLabel);

			var nameLabel = SolLabel((string.IsNullOrEmpty(label) ? prop.displayName : label) + ":")
			 .WithClass(StyleHelper.BgBase02);

			var valueLabel = new Label().WithClass(StyleHelper.FontBold);

			row.Add(nameLabel);
			row.Add(valueLabel);

			row.TrackPropertyValue(prop, Update);
			Update(prop);

			return row;

			void Update(SerializedProperty p)
			{
				var isTrue = p.boolValue;
				valueLabel.text = isTrue ? trueValue : falseValue;
				valueLabel.text = valueLabel.text.PadLeft(1);
				valueLabel.EnableInClassList(StyleHelper.TextGreen, isTrue);
				valueLabel.EnableInClassList(StyleHelper.TextRed, !isTrue);
			}
		}

		public static EnumField SolEnumField(Enum defaultValue, string label = null)
		{
			var field = new EnumField(label, defaultValue);
			field.AddToClassList(StyleHelper.VField);

			return field;
		}

		public static FloatField SolFloatField(
			SerializedProperty serializedProperty = null,
			bool readOnly = false,
			string label = null
		)
		{
			var field = new FloatField(string.IsNullOrEmpty(label) ? serializedProperty?.displayName : label)
			{
				isReadOnly = readOnly,
			};

			field.AddToClassList(StyleHelper.VField);

			if (serializedProperty != null) field.BindProperty(serializedProperty);

			return field;
		}

		public static IntegerField SolIntegerField(string label = null, bool readOnly = false)
		{
			var field = new IntegerField(label)
			{
				isReadOnly = readOnly,
			};

			field.AddToClassList(StyleHelper.VField);

			return field;
		}

		public static ObjectField SolObjectField(string label = null, Type type = null)
		{
			var field = new ObjectField(label);
			if (type != null) field.objectType = type;
			field.AddToClassList(StyleHelper.VField);

			return field;
		}

		public static TextField SolTextField(string label = null, bool readOnly = false)
		{
			var field = new TextField(label)
			{
				isReadOnly = readOnly,
			};

			field.AddToClassList(StyleHelper.VField);

			return field;
		}

		public static Toggle SolToggle(string label = null)
		{
			var field = new Toggle(label);
			field.AddToClassList(StyleHelper.VSwitch);

			return field;
		}

		public static Vector3Field SolVector3Field(string label = null)
		{
			var field = new Vector3Field(label);
			field.AddToClassList(StyleHelper.VField);

			return field;
		}

		public static Vector3IntField SolVector3IntField(string label = null)
		{
			var field = new Vector3IntField(label);
			field.AddToClassList(StyleHelper.VField);

			return field;
		}

#endregion
	}
}