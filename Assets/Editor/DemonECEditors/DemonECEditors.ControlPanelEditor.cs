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

            var health = new SerializedObject(controlPanel.Health);

            var hpValueField = new FloatField
                               {
                                   label = "HP Value"
                               };

            hpValueField.BindProperty(health.FindProperty("HP").FindPropertyRelative("_value"));

            var hpMaxField = new FloatField
                             {
                                 label = "HP Max"
                             };

            hpMaxField.BindProperty(health.FindProperty("HP").FindPropertyRelative("Max"));

            var col = new VisualElement
                      {
                          style =
                          {
                              flexDirection = FlexDirection.Column
                          }
                      };

            col.Add(hpValueField);
            col.Add(hpMaxField);
            var row = Helpers.Editor.Style.Row();
            row.Add(col);
            Helpers.Editor.Style.GenerateDivider(flashlightTestingGroup);
            flashlightTestingGroup.Add(row);
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