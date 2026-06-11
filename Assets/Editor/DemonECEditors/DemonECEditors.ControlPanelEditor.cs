using EC.DemonEC;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DemonECEditors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ControlPanel))]
    public class ControlPanelEditor : UnityEditor.Editor
    {

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var controlPanel = (ControlPanel)target;

            var flashlightTestingGroup = Helpers.Editor.Style.GenerateTestingGroup(
                    "Flashlight Testing",
                    root,
                    new List<(string, EventCallback<ClickEvent>, bool)>
                    {
                        ("Spawn and Test Flashlight", _ => controlPanel.SpawnAndTestFlashlight(),
                         Application.isPlaying && controlPanel.FlashlightPrefab),
                        ("Destroy Flashlight", _ => controlPanel.DestroyFlashlight(),
                         Application.isPlaying && controlPanel.FlashlightPrefab && controlPanel.SpawnedFlashlight)
                    }
                );

            Helpers.Editor.Style.GenerateTestingGroup(
                    "Jumpscare Testing",
                    root,
                    new List<(string, EventCallback<ClickEvent>, bool)>
                    {
                        ("Test Jumpscare", _ => controlPanel.TestJumpscare(),
                         Application.isPlaying && controlPanel.JumpscareTarget),
                        ("Reset Jumpscare", _ => controlPanel.ResetJumpscare(), Application.isPlaying)
                    }
                );

            Helpers.Editor.Style.GenerateTestingGroup(
                    "Pathing Testing",
                    root,
                    new List<(string, EventCallback<ClickEvent>, bool)>
                    {
                        ("Toggle Pathing", _ => controlPanel.TogglePathing(), Application.isPlaying)
                    }
                );

            // 2. Generate default inspector
            InspectorElement.FillDefaultInspector(root, new SerializedObject(controlPanel), this);

            // 3. Creating Property Display Drawers for other components

            // 3a. Generate divider and section label

            Helpers.Editor.Style.GenerateDivider(root);

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

            var subcomponentFoldouts = new List<Foldout>();

            // 3d. Generate Foldouts for subcomponents (GetInitializedSubcomponents() only returns non-null)
            foreach (var comp in subcomponents)
            {
                subcomponentFoldouts.Add(
                        GenerateSubcomponentFoldout(
                                comp,
                                demonController,
                                subcomponents,
                                root
                            )
                    );
            }

            // 4. Attach additional component-based props
            GenerateHP(controlPanel, subcomponentFoldouts, flashlightTestingGroup);

            return root;
        }

        private void GenerateHP(
            ControlPanel controlPanel,
            List<Foldout> subcomponentFoldouts,
            VisualElement flashlightTestingGroup
        )
        {
            if (controlPanel.Health)
            {
                // 1. Set SerializedObject to start pulling props out of
                var health = new SerializedObject(controlPanel.Health);

                // 2. Make new fields and set values equal to the property values
                var hpValueField = new FloatField
                                   {
                                       label = "HP Value",
                                       value = health.FindProperty("HP").FindPropertyRelative("_value").floatValue
                                   };

                var hpMaxField = new FloatField
                                 {
                                     label = "HP Max",
                                     value = health.FindProperty("HP").FindPropertyRelative("Max").floatValue
                                 };

                // 3a. Find the actual component field generated
                // Because this is for a nested property, we have to grab parent that is actually in the class
                var hpOriginal = subcomponentFoldouts.Find(subcomponentFoldout => subcomponentFoldout.name == "Health")
                                                     .Children()
                                                     .ToList()
                                                     .Find(child => child.name == "HP");

                // 3b. Dig in by narrowing VisualElement to PropertyField, which it is 
                if (hpOriginal is PropertyField propertyField)
                {
                    // 3c. schedule.Execute 'spawns' a querying scope to find children
                    // They don't appear under .Children()
                    propertyField.schedule.Execute(() =>
                            {
                                // 3d. Query by name
                                // Found name by logging target of parent's ChangeEvent
                                var hpValueFieldOriginal = propertyField.Q("unity-input-HP._value");

                                // 3e. Dig in by narrowing PropertyField to FloatField
                                if (hpValueFieldOriginal is FloatField valueFloatField)
                                {
                                    // 3f. Register callback for value change from original to new
                                    valueFloatField.RegisterCallback<ChangeEvent<float>>(evt =>
                                            hpValueField.value = evt.newValue
                                        );

                                    // 3g. Register callback for value change from new to original
                                    hpValueField.RegisterCallback<ChangeEvent<float>>(evt =>
                                            valueFloatField.value = evt.newValue
                                        );
                                }

                                // 3d.
                                var hpMaxFieldOriginal = propertyField.Q("unity-input-HP.Max");

                                // 3e.
                                if (hpMaxFieldOriginal is FloatField maxFloatField)
                                {
                                    // 3f.
                                    hpMaxFieldOriginal.RegisterCallback<ChangeEvent<float>>(evt =>
                                            hpMaxField.value = evt.newValue
                                        );

                                    // 3g.
                                    hpMaxField.RegisterCallback<ChangeEvent<float>>(evt =>
                                        maxFloatField.value = evt.newValue
                                    );
                                }
                            }
                        );
                }

                // 3h. Add fields to testing group
                var col = new VisualElement { style = { flexDirection = FlexDirection.Column } };
                col.Add(hpValueField);
                col.Add(hpMaxField);
                // flashlightTestingGroup.Children().First(child => child.name == "row").Add(hpValueField);
                // flashlightTestingGroup.Children().First(child => child.name == "row").Add(hpMaxField);
                flashlightTestingGroup.Children().First(child => child.name == "row").Add(col);

                // 3i. Bind to serialized object
                hpValueField.Bind(health);
            }
        }

        private Foldout GenerateSubcomponentFoldout(
            Component comp,
            Component mainComponent,
            List<Component> subcomponents,
            VisualElement root
        )
        {
            var foldout = new Foldout
                          {
                              text = comp.GetType().Name,
                              name = comp.GetType().Name,
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

            return foldout;
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

                    ele.Add(new PropertyField(prop) { name = prop.name });
                } while (prop.NextVisible(false));
            }

            ele.Bind(nestedSO);
        }

    }
}