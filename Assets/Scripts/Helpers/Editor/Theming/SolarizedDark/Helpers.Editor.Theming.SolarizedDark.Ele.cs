using System;
using Helpers.Editor.Ext;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Helpers.Editor.Theming.SolarizedDark
{
	[AiGenerated("Claude", "Sonnet 4.6")]
	public static class Ele
	{
		public static Label SolLabel() => SolLabel("");

		public static Label SolLabel(string text, bool emphasized = false)
		{
			var label = new Label(text);
			label.AddToClassList(emphasized ? StyleHelper.ClassTextEmphasis : StyleHelper.ClassTextBody);

			return label;
		}

		public static Label SolSecondaryLabel(string text)
		{
			var label = new Label(text);
			label.AddToClassList(StyleHelper.ClassTextSecondary);

			return label;
		}

		public static Button SolButton(string text = "", bool enabled = true)
		{
			var button = new Button
						 {
							 text = text,
						 };

			button.AddToClassList(StyleHelper.ClassButton);
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
			button.AddToClassList(StyleHelper.ClassButton);
			button.SetEnabled(enabled);

			return button;
		}

		// public static Button SolButton(string text = "", EventCallback<ClickEvent> onClick = null, bool enabled = true)
		// {
		// 	var button = new Button
		// 				 {
		// 					 text = text,
		// 				 };
		//
		// 	button.RegisterCallback(onClick);
		// 	button.AddToClassList(StyleHelper.ClassButton);
		// 	button.SetEnabled(enabled);
		//
		// 	return button;
		// }

		public static Button SolPrimaryButton(string text, Action onClick = null, bool enabled = true)
		{
			var button = new Button(onClick)
						 {
							 text = text,
						 };

			button.AddToClassList(StyleHelper.ClassButtonPrimary);

			button.SetEnabled(enabled);

			return button;
		}

		public static VisualElement SolCard()
		{
			var card = new VisualElement();
			card.AddToClassList(StyleHelper.ClassCard);

			return card;
		}

		public static VisualElement SolDivider()
		{
			var divider = new VisualElement();
			divider.AddToClassList(StyleHelper.ClassDivider);

			return divider;
		}

		public static VisualElement SolRow(bool highlighted = false)
		{
			var row = new VisualElement
					  {
						  style =
						  {
							  flexDirection = FlexDirection.Row,
						  },
					  };

			row.AddToClassList(highlighted ? StyleHelper.ClassBgHighlight : StyleHelper.ClassBackground);

			return row;
		}

		public static VisualElement SolCol(bool highlighted = false)
		{
			var col = new VisualElement
					  {
						  style =
						  {
							  flexDirection = FlexDirection.Column,
							  flexGrow = 1,
						  },
					  };

			col.style.SetAllPadding(4);
			col.AddToClassList(highlighted ? StyleHelper.ClassBgHighlight : StyleHelper.ClassBackground);

			return col;
		}

		// Root container — call this instead of manually applying the stylesheet
		public static VisualElement SolRoot()
		{
			var root = new VisualElement();
			StyleHelper.ApplyTo(root);
			root.AddToClassList(StyleHelper.ClassBackground);
			root.style.flexGrow = 1;

			return root;
		}

		public static Toggle SolToggle(string label = null)
		{
			var field = new Toggle(label);
			field.AddToClassList(StyleHelper.ClassToggle);

			return field;
		}

		public static IntegerField SolIntegerField(string label = null, bool readOnly = false)
		{
			var field = new IntegerField(label)
						{
							isReadOnly = readOnly,
						};

			field.AddToClassList(StyleHelper.ClassInputField);

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

			field.AddToClassList(StyleHelper.ClassInputField);

			var fieldLabel = field.Q<Label>();

			if (fieldLabel != null)
			{
				fieldLabel.style.minWidth = 0;
				fieldLabel.style.flexShrink = 1;
			}

			if (serializedProperty != null) field.BindProperty(serializedProperty);

			return field;
		}

		public static FloatField SolFloatField(
			SerializedProperty serializedProperty = null,
			string label = null,
			bool readOnly = false
		) =>
			SolFloatField(serializedProperty, readOnly, label);

		public static TextField SolTextField(string label = null, bool readOnly = false)
		{
			var field = new TextField(label)
						{
							isReadOnly = readOnly,
						};

			field.AddToClassList(StyleHelper.ClassInputField);

			return field;
		}

		public static Vector3Field SolVector3Field(string label = null)
		{
			var field = new Vector3Field(label);
			field.AddToClassList(StyleHelper.ClassInputField);

			return field;
		}

		public static Vector3IntField SolVector3IntField(string label = null)
		{
			var field = new Vector3IntField(label);
			field.AddToClassList(StyleHelper.ClassInputField);

			return field;
		}

		public static ObjectField SolObjectField(string label = null, Type type = null)
		{
			var field = new ObjectField(label);
			if (type != null) field.objectType = type;
			field.AddToClassList(StyleHelper.ClassInputField);

			return field;
		}

		public static EnumField SolEnumField(Enum defaultValue, string label = null)
		{
			var field = new EnumField(label, defaultValue);
			field.AddToClassList(StyleHelper.ClassInputField);

			return field;
		}

		public static VisualElement SolGrid([ItemCanBeNull] VisualElement[][] gridItems, string label = null)
		{
			var card = SolCard();

			card.Add(SolLabel(label, true));

			AppendSolGrid(card, gridItems);

			return card;
		}

		public static void AppendSolGrid(VisualElement solGrid, [ItemCanBeNull] VisualElement[][] gridItems)
		{
			foreach (var gridRow in gridItems)
			{
				var row = SolRow(true);

				if (gridRow != null)
					foreach (var gridItem in gridRow)
					{
						var col = SolCol(true);

						if (gridItem != null) col.Add(gridItem);

						row.Add(col);
					}

				solGrid.Add(row);
			}
		}

		public static Label SolBooleanLabel(
			SerializedProperty prop,
			string trueValue = "True",
			string falseValue = "False",
			string label = null
		)
		{
			var booleanLabel = SolSecondaryLabel($"{(string.IsNullOrEmpty(label) ? prop.displayName : label)}:");

			booleanLabel.TrackPropertyValue(prop, SetLabelTextFromBool);
			SetLabelTextFromBool(prop);

			return booleanLabel;

			void SetLabelTextFromBool(SerializedProperty p)
			{
				booleanLabel.text = $"{label}: {(p.boolValue ? trueValue : falseValue)}";
			}
		}
	}
}