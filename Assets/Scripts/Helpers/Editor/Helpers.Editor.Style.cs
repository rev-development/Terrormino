using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable MemberCanBePrivate.Global

namespace Helpers.Editor
{
    public static class Style
    {

        public static StyleColor LightGrey = RGBAtoStyleColor(70f);

        public static StyleColor NearBlack = RGBAtoStyleColor(26f);

        public static StyleColor NearWhite = RGBAtoStyleColor(210f);

        public static StyleColor RGBAtoStyleColor(float r, float g, float b, float a = 255f) {
            return new StyleColor(
                    new Color(
                            r / 255f,
                            g / 255f,
                            b / 255f,
                            a / 255f
                        )
                );
        }

        public static StyleColor RGBAtoStyleColor(float rgb, float a = 255f) {
            return RGBAtoStyleColor(
                    rgb,
                    rgb,
                    rgb,
                    a
                );
        }

        public static void SetAllBorderRadius(VisualElement ele, int value) {
            ele.style.borderTopLeftRadius = value;
            ele.style.borderTopRightRadius = value;
            ele.style.borderBottomLeftRadius = value;
            ele.style.borderBottomRightRadius = value;
        }

        public static void SetAllPadding(VisualElement ele, int value) {
            ele.style.paddingLeft = value;
            ele.style.paddingBottom = value;
            ele.style.paddingTop = value;
            ele.style.paddingRight = value;
        }

        public static void SetAllBorder(VisualElement ele, int width, StyleColor styleColor) {
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
        ) {
            var button = new Button
                         {
                             text = buttonText,
                             style =
                             {
                                 backgroundColor = Solarized.Cyan
                             }
                         };

            SetAllBorder(button, 1, Solarized.Base00);
            button.RegisterCallback(clickHandler);
            container.Add(button);
            button.SetEnabled(enabledCondition);

            return button;
        }

        public static VisualElement Row() {
            return new VisualElement
                   {
                       style =
                       {
                           flexDirection = FlexDirection.Row
                       }
                   };
        }

        public static VisualElement GenerateDivider(VisualElement container) {
            VisualElement divider = new()
                                    {
                                        style =
                                        {
                                            height = 1,
                                            marginTop = 4,
                                            marginBottom = 4,
                                            backgroundColor = Solarized.Base00
                                        }
                                    };

            container.Add(divider);

            return divider;
        }

        public static VisualElement GenerateGroup() {
            var group = new VisualElement
                        {
                            style =
                            {
                                backgroundColor = Solarized.Base03,
                                flexGrow = 1
                            }
                        };

            SetAllBorderRadius(group, 3);
            SetAllPadding(group, 4);
            SetAllBorder(group, 1, Solarized.Base00);

            return group;
        }

        public static VisualElement GenerateTestingGroup(
            string labelText,
            VisualElement container,
            List<(string, EventCallback<ClickEvent>, bool)> buttonParams
        ) {
            // 1. Creating Custom Buttons

            // 1a. Generate Group for Controls
            var group = GenerateGroup();
            group.style.marginBottom = 4;

            var label = new Label
                        {
                            text = labelText,
                            style =
                            {
                                unityFontStyleAndWeight = FontStyle.Bold
                            }
                        };

            group.Add(label);

            // 1b. Generate Row for Buttons
            var row = new VisualElement
                      {
                          name = "row",
                          style =
                          {
                              flexDirection = FlexDirection.Row,
                              flexGrow = 1
                          }
                      };

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

        // public static void BindNestedField<TField, TValue>(
        //     PropertyField sourcePropertyField,
        //     string unityInputQueryName,
        //     TField mirrorField
        // )
        //     where TField : BaseField<TValue> {
        //     sourcePropertyField.schedule.Execute(() =>
        //             {
        //                 var source = sourcePropertyField.Q(unityInputQueryName);
        //
        //                 if (source is not TField sourceField) return;
        //
        //                 sourceField.RegisterCallback<ChangeEvent<TValue>>(evt => mirrorField.value = evt.newValue);
        //                 mirrorField.RegisterCallback<ChangeEvent<TValue>>(evt => sourceField.value = evt.newValue);
        //             }
        //         );
        // }

        public static class Solarized
        {

            public static StyleColor Base03 = RGBAtoStyleColor(0f, 43f, 54f);

            public static StyleColor Base02 = RGBAtoStyleColor(7f, 54f, 66f);

            public static StyleColor Base01 = RGBAtoStyleColor(88f, 110f, 117f);

            public static StyleColor Base00 = RGBAtoStyleColor(101f, 123f, 131f);

            public static StyleColor Yellow = RGBAtoStyleColor(181f, 137f, 0f);

            public static StyleColor Orange = RGBAtoStyleColor(203f, 75f, 22f);

            public static StyleColor Red = RGBAtoStyleColor(211f, 1f, 2f);

            public static StyleColor Magenta = RGBAtoStyleColor(211f, 54f, 130f);

            public static StyleColor Violet = RGBAtoStyleColor(108f, 113f, 196f);

            public static StyleColor Blue = RGBAtoStyleColor(38f, 139f, 210f);

            public static StyleColor Cyan = RGBAtoStyleColor(42f, 161f, 152f);

            public static StyleColor Green = RGBAtoStyleColor(133f, 153f, 0f);

        }

    }
}