using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DemonECEditors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EC.DemonEC.ControlPanel))]
    public class ControlPanelEditor : UnityEditor.Editor
    {

        private readonly StyleColor _lightGrey = new(
                new Color(
                        70f / 255f,
                        70f / 255f,
                        70f / 255f,
                        1f
                    )
            );
        private readonly StyleColor _nearBlack = new(
                new Color(
                        26f / 255f,
                        26f / 255f,
                        26f / 255f,
                        1f
                    )
            );
        private readonly StyleColor _nearWhite = new(
                new Color(
                        210f / 255f,
                        210f / 255f,
                        210f / 255f,
                        1f
                    )
            );

        private Button GenerateButton(
            string buttonText,
            EventCallback<ClickEvent> clickHandler,
            VisualElement container
        )
        {
            var button = new Button { text = buttonText };
            button.RegisterCallback(clickHandler);
            container.Add(button);

            return button;
        }

        private void GenerateDivider(VisualElement container)
        {
            VisualElement divider = new()
                                    {
                                        style =
                                        {
                                            height = 1,
                                            marginTop = 10,
                                            marginBottom = 10,
                                            backgroundColor = _nearWhite
                                        }
                                    };

            container.Add(divider);
        }

        private void SetAllBorderRadius(VisualElement ele, int value)
        {
            ele.style.borderTopLeftRadius = value;
            ele.style.borderTopRightRadius = value;
            ele.style.borderBottomLeftRadius = value;
            ele.style.borderBottomRightRadius = value;
        }

        private void SetAllPadding(VisualElement ele, int value)
        {
            ele.style.paddingLeft = value;
            ele.style.paddingBottom = value;
            ele.style.paddingTop = value;
            ele.style.paddingRight = value;
        }

        private void SetAllBorder(VisualElement ele, int width, StyleColor styleColor)
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

        private VisualElement GenerateGroup()
        {
            var group = new VisualElement { style = { backgroundColor = _lightGrey } };
            SetAllBorderRadius(group, 3);
            SetAllPadding(group, 4);
            SetAllBorder(group, 1, _nearBlack);

            return group;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var controlPanel = (EC.DemonEC.ControlPanel)target;

            // 1. Creating Custom Buttons

            // 1a. Generate Group for Controls
            var flashlightTestingGroup = GenerateGroup();

            var flashlightTestingLabel = new Label
                                         {
                                             text = "Flashlight Testing",
                                             style = { unityFontStyleAndWeight = FontStyle.Bold }
                                         };

            flashlightTestingGroup.Add(flashlightTestingLabel);

            // 1b. Generate Row for Buttons
            var flashlightTestingRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            // 1c. Generate specific Buttons with their callbacks and enabled conditions
            var spawnAndTestFlashlightButton = GenerateButton(
                    "Spawn and Test Flashlight",
                    _ => controlPanel.SpawnAndTestFlashlight(),
                    flashlightTestingRow
                );

            spawnAndTestFlashlightButton.SetEnabled(Application.isPlaying && controlPanel.FlashlightPrefab);

            var destroyTestFlashlightButton = GenerateButton(
                    "Destroy Flashlight",
                    _ => controlPanel.DestroyFlashlight(),
                    flashlightTestingRow
                );

            destroyTestFlashlightButton.SetEnabled(
                    Application.isPlaying && controlPanel.FlashlightPrefab && controlPanel.SpawnedFlashlight
                );

            // 1d. Add row to group and group to root
            flashlightTestingGroup.Add(flashlightTestingRow);

            root.Add(flashlightTestingGroup);

            // 1e. Repeat for additional groups

            var jumpscareTestingGroup = GenerateGroup();

            var jumpscareTestingLabel = new Label { text = "Jumpscare Testing" };

            jumpscareTestingGroup.Add(jumpscareTestingLabel);

            var jumpscareTestingRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            var testJumpscareButton = GenerateButton(
                    "Test Jumpscare",
                    _ => controlPanel.TestJumpscare(),
                    jumpscareTestingRow
                );

            testJumpscareButton.SetEnabled(Application.isPlaying && controlPanel.JumpscareTarget);

            var resetJumpscareButton = GenerateButton(
                    "Reset Jumpscare",
                    _ => controlPanel.ResetJumpscare(),
                    jumpscareTestingRow
                );

            resetJumpscareButton.SetEnabled(Application.isPlaying);

            jumpscareTestingGroup.Add(jumpscareTestingRow);

            root.Add(jumpscareTestingGroup);

            // 2. Generate default inspector
            InspectorElement.FillDefaultInspector(root, new SerializedObject(controlPanel), this);


            // 3. Creating Property Display Drawers for other components

            // 3a. Generate divider and section label

            GenerateDivider(root);

            var componentLabel = new Label
                                 {
                                     text = "Demon Components", style = { unityFontStyleAndWeight = FontStyle.Bold }
                                 };

            root.Add(componentLabel);

            // 3b. Get main component and then subcomponents
            var demonController = controlPanel.GetInitializedMainComponent();

            var subcomponents = controlPanel.GetInitializedSubcomponents();

            // 3c. Generate Foldout for main component (escape if missing)
            if (!demonController)
            {
                return root;
            }

            GenerateSubcomponentFoldout(
                    demonController,
                    demonController,
                    subcomponents,
                    root
                );

            // 3d. Generate Foldouts for subcomponents (GetInitializedSubcomponents() only returns non-null)
            foreach (var comp in subcomponents)
            {
                GenerateSubcomponentFoldout(
                        comp,
                        demonController,
                        subcomponents,
                        root
                    );
            }

            return root;
        }

        private void GenerateSubcomponentFoldout(
            Component comp,
            Component mainComponent,
            List<Component> subcomponents,
            VisualElement root
        )
        {
            var foldout = new Foldout
                          {
                              text = comp.GetType().Name,
                              viewDataKey = $"{mainComponent.gameObject.GetInstanceID()}_{comp.GetType().Name}_Foldout"
                          };

            IterateProps(
                    comp,
                    foldout,
                    new[]
                        {
                            mainComponent
                        }.Concat(subcomponents)
                         .ToArray()
                );

            root.Add(foldout);
        }

        private void IterateProps(
            Component comp,
            VisualElement ele,
            params Component[] ignoreReferencesToTheseComponents
        )
        {
            SerializedObject nestedSO = new(comp);

            var prop = nestedSO.GetIterator();

            if (prop.NextVisible(true))
            {
                do
                {
                    // Skip if:
                    // Default Script Property
                    // Reference to Subcomponent being Iterated
                    // Reference to the Control Panel
                    if (prop.name == "m_Script"
                        || (prop.propertyType == SerializedPropertyType.ObjectReference
                            && (ignoreReferencesToTheseComponents.Contains(prop.objectReferenceValue)
                                || prop.objectReferenceValue == target)))
                    {
                        continue;
                    }

                    ele.Add(new PropertyField(prop));
                } while (prop.NextVisible(false));
            }

            ele.Bind(nestedSO);
        }

    }
}