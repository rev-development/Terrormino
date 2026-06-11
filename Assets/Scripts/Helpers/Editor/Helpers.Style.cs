using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable MemberCanBePrivate.Global

namespace Helpers.Editor
{
    public static class Style
    {

        public static StyleColor LightGrey = new(
                new Color(
                        70f / 255f,
                        70f / 255f,
                        70f / 255f,
                        1f
                    )
            );
        public static StyleColor NearBlack = new(
                new Color(
                        26f / 255f,
                        26f / 255f,
                        26f / 255f,
                        1f
                    )
            );
        public static StyleColor NearWhite = new(
                new Color(
                        210f / 255f,
                        210f / 255f,
                        210f / 255f,
                        1f
                    )
            );

        public static void SetAllBorderRadius(VisualElement ele, int value)
        {
            ele.style.borderTopLeftRadius = value;
            ele.style.borderTopRightRadius = value;
            ele.style.borderBottomLeftRadius = value;
            ele.style.borderBottomRightRadius = value;
        }

        public static void SetAllPadding(VisualElement ele, int value)
        {
            ele.style.paddingLeft = value;
            ele.style.paddingBottom = value;
            ele.style.paddingTop = value;
            ele.style.paddingRight = value;
        }

        public static void SetAllBorder(VisualElement ele, int width, StyleColor styleColor)
        {
            ele.style.borderLeftWidth = width;
            ele.style.borderLeftColor = styleColor;

            ele.style.borderTopWidth = width;
            ele.style.borderTopColor = styleColor;

            ele.style.borderRightWidth = width;
            ele.style.borderRightColor = styleColor;

            ele.style.borderBottomWidth = width;
            ele.style.borderBottomColor = styleColor;
        }

        public static Button GenerateButton(
            string buttonText,
            EventCallback<ClickEvent> clickHandler,
            VisualElement container,
            bool enabledCondition
        )
        {
            var button = new Button { text = buttonText };
            button.RegisterCallback(clickHandler);
            container.Add(button);
            button.SetEnabled(enabledCondition);

            return button;
        }

        public static VisualElement Row()
        {
            return new VisualElement { style = { flexDirection = FlexDirection.Row } };
        }

        public static VisualElement GenerateDivider(VisualElement container)
        {
            VisualElement divider = new()
                                    {
                                        style =
                                        {
                                            height = 1,
                                            marginTop = 10,
                                            marginBottom = 10,
                                            backgroundColor = NearWhite
                                        }
                                    };

            container.Add(divider);

            return divider;
        }

        public static VisualElement GenerateGroup()
        {
            var group = new VisualElement { style = { backgroundColor = LightGrey } };
            SetAllBorderRadius(group, 3);
            SetAllPadding(group, 4);
            SetAllBorder(group, 1, NearBlack);

            return group;
        }

        public static VisualElement GenerateTestingGroup(
            string labelText,
            VisualElement container,
            List<(string, EventCallback<ClickEvent>, bool)> buttonParams
        )
        {
            // 1. Creating Custom Buttons

            // 1a. Generate Group for Controls
            var group = GenerateGroup();

            var label = new Label { text = labelText, style = { unityFontStyleAndWeight = FontStyle.Bold } };

            group.Add(label);

            // 1b. Generate Row for Buttons
            var row = new VisualElement { name = "row", style = { flexDirection = FlexDirection.Row } };

            // 1c. Generate specific Buttons with their callbacks and enabled conditions
            foreach (var buttonParam in buttonParams)
            {
                GenerateButton(
                        buttonParam.Item1,
                        buttonParam.Item2,
                        row,
                        buttonParam.Item3
                    );
            }

            // 1d. Add row to group and group to root
            group.Add(row);

            container.Add(group);

            return group;
        }

    }
}