using System.Collections.Generic;
using System.Linq;
using EC.DemonEC;
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

        public override VisualElement CreateInspectorGUI() {
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
                        ("Path to Jumpscare Target", _ => controlPanel.TestJumpscare(),
                         Application.isPlaying && controlPanel.JumpscareTarget),
                        ("Reset Jumpscare", _ => controlPanel.ResetJumpscare(), Application.isPlaying)
                    }
                );

            var pathingTestingGroup = Helpers.Editor.Style.GenerateTestingGroup(
                    "Pathing Testing",
                    root,
                    new List<(string, EventCallback<ClickEvent>, bool)>
                    {
                        ("Toggle Pathing", _ => controlPanel.TogglePathing(), Application.isPlaying)
                    }
                );

            Helpers.Editor.Style.GenerateTestingGroup(
                    "Config Tools",
                    root,
                    new List<(string, EventCallback<ClickEvent>, bool)>
                    {
                        ("Initialize Components", _ => controlPanel.GetInitializedComponents(), !Application.isPlaying)
                    }
                );

            var controlPanelSO = new SerializedObject(controlPanel);

            // 2. Generate default inspector
            InspectorElement.FillDefaultInspector(root, controlPanelSO, this);

            // 3. Creating Property Display Drawers for other components

            // 3a. Generate divider and section label

            Helpers.Editor.Style.GenerateDivider(root);

            var componentLabel = new Label
                                 {
                                     text = "Demon Components",
                                     style =
                                     {
                                         unityFontStyleAndWeight = FontStyle.Bold
                                     }
                                 };

            root.Add(componentLabel);

            var components = controlPanel.GetInitializedComponents();
            // 3d. Generate Foldouts for subcomponents (GetInitializedSubcomponents() only returns non-null)

            var subcomponentFoldouts = components.Select(comp => GenerateSubcomponentFoldout(
                                                          comp,
                                                          controlPanel.gameObject,
                                                          components,
                                                          root
                                                      )
                                                  )
                                                 .ToList();

            // 4. Attach additional component-based props
            GenerateHP(controlPanel, subcomponentFoldouts, flashlightTestingGroup);
            GenerateNavBeacons(controlPanel, pathingTestingGroup);

            return root;
        }

        private void GenerateNavBeacons(ControlPanel controlPanel, VisualElement pathingTestingGroup) {
            Helpers.Editor.Style.GenerateDivider(pathingTestingGroup);

            var navBeaconLabel = new Label
                                 {
                                     text = "Nav Beacons",
                                     style =
                                     {
                                         unityFontStyleAndWeight = FontStyle.Bold,
                                         marginBottom = 2
                                     }
                                 };

            pathingTestingGroup.Add(navBeaconLabel);

            var navBeaconRow = Helpers.Editor.Style.Row();
            navBeaconRow.style.flexGrow = 1;
            pathingTestingGroup.Add(navBeaconRow);

            var navBeaconList = new ListView
                                {
                                    itemsSource = controlPanel.NavBeacons,
                                    makeItem = () =>
                                    {
                                        var row = Helpers.Editor.Style.Row();
                                        row.name = "row";

                                        var label = new Label
                                                    {
                                                        name = "label"
                                                    };

                                        row.Add(label);

                                        var button = new Button
                                                     {
                                                         name = "btn"
                                                     };

                                        row.Add(button);

                                        return row;
                                    },
                                    bindItem = (element, index) =>
                                    {
                                        element.schedule.Execute(() =>
                                                {
                                                    var qLabel = element.Q("label");

                                                    if (qLabel is Label label)
                                                    {
                                                        label.text = controlPanel.NavBeacons[index].name;
                                                        label.style.unityTextAlign = TextAnchor.MiddleLeft;
                                                    }

                                                    var qBtn = element.Q("btn");

                                                    if (qBtn is Button btn)
                                                    {
                                                        btn.text = "Go To";
                                                        btn.style.backgroundColor = Helpers.Editor.Style.Solarized.Cyan;

                                                        btn.RegisterCallback<ClickEvent>(evt =>
                                                                controlPanel.Pathing.GoTo(
                                                                    controlPanel.NavBeacons[index]
                                                                )
                                                            );

                                                        btn.SetEnabled(Application.isPlaying);
                                                    }
                                                }
                                            );
                                    },
                                    showAddRemoveFooter = false, // Disable modifications
                                    reorderable = false,
                                    style =
                                    {
                                        backgroundColor = Helpers.Editor.Style.Solarized.Base03,
                                        flexGrow = 1
                                    }
                                };

            navBeaconRow.Add(navBeaconList);
        }

        private void GenerateHP(
            ControlPanel controlPanel,
            List<Foldout> subcomponentFoldouts,
            VisualElement flashlightTestingGroup
        ) {
            if (controlPanel.Health == null) return;

            var hpSubcomponent = subcomponentFoldouts.Find(subcomponentFoldout => subcomponentFoldout.name == "Health");

            if (hpSubcomponent == null) return;

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

            var hpOriginal = hpSubcomponent.Children().ToList().Find(child => child.name == "HP");

            // 3b. Dig in by narrowing VisualElement to PropertyField, which it is 
            if (hpOriginal is PropertyField propertyField)
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

            // 3h. Add fields to testing group
            var col = new VisualElement
                      {
                          style =
                          {
                              flexDirection = FlexDirection.Column
                          }
                      };

            col.Add(hpValueField);
            col.Add(hpMaxField);
            // flashlightTestingGroup.Children().First(child => child.name == "row").Add(hpValueField);
            // flashlightTestingGroup.Children().First(child => child.name == "row").Add(hpMaxField);
            var row = Helpers.Editor.Style.Row();
            row.Add(col);
            Helpers.Editor.Style.GenerateDivider(flashlightTestingGroup);
            flashlightTestingGroup.Add(row);

            // 3i. Bind to serialized object
            hpValueField.Bind(health);
        }

        private Foldout GenerateSubcomponentFoldout(
            Component comp,
            GameObject mainGO,
            List<Component> subcomponents,
            VisualElement root
        ) {
            var foldout = new Foldout
                          {
                              text = comp.GetType().Name,
                              name = comp.GetType().Name,
                              viewDataKey = $"{mainGO.GetInstanceID()}_{comp.GetType().Name}_Foldout"
                          };

            IterateProps(comp, foldout, subcomponents.ToArray());

            root.Add(foldout);

            return foldout;
        }

        private void IterateProps(
            Component comp,
            VisualElement ele,
            params Component[] ignoreReferencesToTheseComponents
        ) {
            SerializedObject nestedSO = new(comp);

            var prop = nestedSO.GetIterator();

            if (prop.NextVisible(true))
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
                        continue;

                    ele.Add(
                            new PropertyField(prop)
                            {
                                name = prop.name
                            }
                        );
                } while (prop.NextVisible(false));

            ele.Bind(nestedSO);
        }

    }
}