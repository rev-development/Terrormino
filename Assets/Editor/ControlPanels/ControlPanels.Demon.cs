using System;
using System.Collections.Generic;
using System.Linq;
using EC.Demon;
using EC.Demon.Pathing;
using Helpers.Editor.Ext;
using Helpers.Editor.Theming.SolarizedDark;
using Helpers.Ext;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Helpers.Editor.Theming.SolarizedDark.Ele;

namespace Editor.ControlPanels
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ControlPanel))]
	public class Demon : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var root = SolRoot();

			var controlPanel = (ControlPanel)target;

			var flashlightTestingPanel = SolGrid(
				"Flashlight Testing",
				new VisualElement[]
				{
					SolButton(
						_ => controlPanel.SpawnAndTestFlashlight(),
						"Spawn & Test Flashlight",
						Application.isPlaying
					),
					SolButton(_ => controlPanel.DestroyFlashlight(), "Destroy Flashlight", Application.isPlaying),
				}
			);

			root.Add(flashlightTestingPanel);

			var jumpscareTestingPanel = SolGrid(
				"Jumpscare Testing",
				new VisualElement[]
				{
					SolButton(
						_ => controlPanel.PathToJumpscareTarget(),
						"Path to Jumpscare Target",
						Application.isPlaying
					),
					SolButton(_ => controlPanel.ResetJumpscare(), "Fully Reset Jumpscare", Application.isPlaying),
				},
				new VisualElement[]
				{
					SolButton(_ => controlPanel.PositionForJumpscare(), "Position for Jumpscare"),
					SolButton(_ => controlPanel.RevertPositionFromJumpscare(), "Revert Position from Jumpscare"),
				}
			);

			root.Add(jumpscareTestingPanel);

			var pathingTestingPanel = SolGrid(
				"Pathing Testing",
				new VisualElement[]
				{
					SolButton(_ => controlPanel.TogglePathing(), "Toggle Pathing", Application.isPlaying),
					SolButton(
						_ => controlPanel.Pathing.EnterState(StateType.Patrol),
						"Enter State: Patrol",
						Application.isPlaying
					),
					SolButton(
						_ => controlPanel.Pathing.EnterState(StateType.Chase),
						"Enter State: Chase",
						Application.isPlaying
					),
				}
			);

			root.Add(pathingTestingPanel);

			var configTestingPanel = SolGrid(
				"Config",
				new VisualElement[]
				{
					SolButton(
						_ => controlPanel.GetInitializedComponents(),
						"Initialize Components",
						!Application.isPlaying
					),
				}
			);

			root.Add(configTestingPanel);

			var controlPanelSO = new SerializedObject(controlPanel);

			// 2. Generate default inspector
			InspectorElement.FillDefaultInspector(root, controlPanelSO, this);

			// 3. Creating Property Display Drawers for other components

			// 3a. Generate divider and section label

			root.Add(SolDivider());

			var componentLabel = SolLabel("Demon Components");

			root.Add(componentLabel);

			var subSOs = controlPanel.GetInitializedComponents().Select(comp => new SerializedObject(comp)).ToList();
			// 3d. Generate Foldouts for subcomponents (GetInitializedSubcomponents() only returns non-null)

			var subcomponentFoldouts = subSOs
									  .Select(so => GenerateSubcomponentFoldout(
											   so,
											   controlPanel.gameObject,
											   subSOs,
											   root
										   )
									   )
									  .ToList();

			// // 4. Attach additional component-based props
			GenerateHP(controlPanel, subcomponentFoldouts, flashlightTestingPanel);
			GenerateCurrentStateType(controlPanel, subcomponentFoldouts, pathingTestingPanel);
			GenerateNavBeacons(controlPanel, pathingTestingPanel);

			return root;
		}

		private void GenerateNavBeacons(ControlPanel controlPanel, VisualElement pathingTestingGroup)
		{
			pathingTestingGroup.Add(SolDivider());

			var navBeaconLabel = SolLabel("Nav Beacons");

			pathingTestingGroup.Add(navBeaconLabel);

			var navBeaconCol = SolCol();

			pathingTestingGroup.Add(navBeaconCol);

			var navBeaconList = SolList(
				controlPanel.NavBeacons,
				makeItem: () =>
				{
					var row = SolRow(true)
					   .WithStyle(r =>
							{
								r.marginLeft = 0;
								r.marginLeft = 0;
							}
						);

					row.name = "row";

					var label = SolLabel();
					label.name = "label";

					row.Add(label);

					var button = SolButton();

					button.name = "btn";

					row.Add(button);

					return row;
				},
				bindItem: (element, index) =>
				{
					element.schedule.Execute(() =>
						{
							var qLabel = element.Q("label");

							if (qLabel is Label label) label.text = controlPanel.NavBeacons[index].name;

							var qBtn = element.Q("btn");

							if (qBtn is Button btn)
							{
								btn.text = "Go To";

								btn.RegisterCallback<ClickEvent>(_ =>
									controlPanel.Pathing.NavMeshAgent.GoTo(
										controlPanel.NavBeacons[index].transform.position
									)
								);

								btn.SetEnabled(Application.isPlaying);
							}
						}
					);
				}
			);

			navBeaconList.showAddRemoveFooter = false;
			navBeaconList.reorderable = false;

			navBeaconCol.Add(navBeaconList);
		}

		private void GenerateCurrentStateType(
			ControlPanel controlPanel,
			List<Foldout> subcomponentFoldouts,
			VisualElement pathingTestingPanel
		)
		{
			if (controlPanel.Pathing == null) return;

			var pathingSubcomponent = subcomponentFoldouts.Find(subcomponentFoldout =>
				subcomponentFoldout.name == "EC.Demon.Pathing.Controller"
			);

			if (pathingSubcomponent == null) return;

			var pathing = new SerializedObject(controlPanel.Pathing);

			var currentStateTypeLabel = SolLabel(
				"Current State Type: "
				+ (Enum.GetNames(typeof(StateType)).Length > pathing.FindProperty("CurrentStateType").enumValueIndex
				   && pathing.FindProperty("CurrentStateType").enumValueIndex >= 0
					? Enum.GetNames(typeof(StateType))[pathing.FindProperty("CurrentStateType").enumValueIndex]
					: "Undefined")
			);

			currentStateTypeLabel.TrackPropertyValue(
				pathing.FindProperty("CurrentStateType"),
				p => currentStateTypeLabel.text = "Current State Type: "
												  + (Enum.GetNames(typeof(StateType)).Length > p.enumValueIndex
													 && p.enumValueIndex >= 0
													  ? Enum.GetNames(typeof(StateType))[p.enumValueIndex]
													  : "Undefined")
			);

			pathingTestingPanel.Add(SolDivider());

			AppendSolGrid(
				pathingTestingPanel,
				new VisualElement[]
				{
					currentStateTypeLabel,
				}
			);

			currentStateTypeLabel.Bind(pathing);
		}

		private void GenerateHP(
			ControlPanel controlPanel,
			List<Foldout> subcomponentFoldouts,
			VisualElement flashlightTestingPanel
		)
		{
			if (controlPanel.Health == null) return;

			var hpSubcomponent
				= subcomponentFoldouts.Find(subcomponentFoldout => subcomponentFoldout.name == "EC.Demon.Health");

			if (hpSubcomponent == null) return;

			var health = new SerializedObject(controlPanel.Health);

			var hpValueField = SolFloatField(health.FindProperty("HP").FindPropertyRelative("_value"), "HP Value");

			var hpMaxField = SolFloatField(health.FindProperty("HP").FindPropertyRelative("Max"), "HP Max");

			flashlightTestingPanel.Add(SolDivider());

			AppendSolGrid(
				flashlightTestingPanel,
				new VisualElement[]
				{
					hpValueField,
					hpMaxField,
				}
			);

			hpValueField.Bind(health);
		}

		private Foldout GenerateSubcomponentFoldout(
			SerializedObject so,
			GameObject mainGO,
			List<SerializedObject> subSOs,
			VisualElement root
		)
		{
			var foldout = SolFoldout(so.targetObject.GetType().FullName);
			foldout.name = so.targetObject.GetType().FullName;
			foldout.viewDataKey = $"{mainGO.GetInstanceID()}_{so.targetObject.GetType().Name}_Foldout";

			so.IterateProps(
				foldout,
				prop => prop.name == "m_Script"
						|| (prop.propertyType == SerializedPropertyType.ObjectReference
							&& (subSOs.Select(subSO => subSO.targetObject).Contains(prop.objectReferenceValue)
								|| prop.objectReferenceValue == target)),
				new[]
				{
					StyleHelper.VField,
				}
			);

			root.Add(foldout);

			return foldout;
		}
	}
}