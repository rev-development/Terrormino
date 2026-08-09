using System;
using System.Collections.Generic;
using System.Linq;
using EC.Demon;
using EC.Demon.Pathing;
using Helpers.Editor;
using Helpers.Editor.Ext;
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
	public class Demon : ControlPanelDrawerBase<ControlPanel>
	{
		protected override List<Func<ControlPanel, VisualElement>> _customPanelGenerationFunctions =>
			new()
			{
				GenerateFlashlightPanel,
				GenerateJumpscarePanel,
				GeneratePathingPanel,
			};

		public override VisualElement CreateInspectorGUI()
		{
			var root = SolRoot();
			var controlPanel = (ControlPanel)target;
			var subSOs = controlPanel.GetInitializedComponents().Select(comp => new SerializedObject(comp)).ToList();

			root.Add(GenerateFlashlightPanel(controlPanel));
			root.Add(GenerateJumpscarePanel(controlPanel));
			root.Add(GeneratePathingPanel(controlPanel));
			root.Add(GenerateConfigPanel(controlPanel));

			var controlPanelSO = new SerializedObject(controlPanel);
			InspectorElement.FillDefaultInspector(root, controlPanelSO, this);

			root.Add(SolDivider());
			root.Add(SolLabel("Demon Components"));

			subSOs.Select(so => GenerateComponentFoldout(so, controlPanel.gameObject, subSOs)).ToList().ForEach(root.Add);

			return root;
		}

		private VisualElement GenerateFlashlightPanel(ControlPanel controlPanel)
		{
			var panel = SolGrid(
				"Flashlight Testing",
				new VisualElement[]
				{
					SolButton(_ => controlPanel.SpawnAndTestFlashlight(), "Spawn & Test Flashlight", Application.isPlaying),
					SolButton(_ => controlPanel.DestroyFlashlight(), "Destroy Flashlight", Application.isPlaying),
				}
			);

			GenerateHP(controlPanel, panel);

			return panel;
		}

		private VisualElement GenerateJumpscarePanel(ControlPanel controlPanel) =>
			SolGrid(
				"Jumpscare Testing",
				new VisualElement[]
				{
					SolButton(_ => controlPanel.PathToJumpscareTarget(), "Path to Jumpscare Target", Application.isPlaying),
					SolButton(_ => controlPanel.ResetJumpscare(), "Fully Reset Jumpscare", Application.isPlaying),
				},
				new VisualElement[]
				{
					SolButton(_ => controlPanel.PositionForJumpscare(), "Position for Jumpscare"),
					SolButton(_ => controlPanel.RevertPositionFromJumpscare(), "Revert Position from Jumpscare"),
				}
			);

		private VisualElement GeneratePathingPanel(ControlPanel controlPanel)
		{
			var panel = SolGrid(
				"Pathing Testing",
				new VisualElement[]
				{
					SolButton(_ => controlPanel.TogglePathing(), "Toggle Pathing", Application.isPlaying),
					SolButton(
						_ => controlPanel.Pathing.EnterState(StateType.Patrol),
						"Enter State: Patrol",
						Application.isPlaying
					),
					SolButton(_ => controlPanel.Pathing.EnterState(StateType.Chase), "Enter State: Chase", Application.isPlaying),
				}
			);

			GenerateCurrentStateType(controlPanel, panel);
			GenerateNavBeacons(controlPanel, panel);

			return panel;
		}

		private void GenerateNavBeacons(ControlPanel controlPanel, VisualElement pathingTestingGroup)
		{
			pathingTestingGroup.Add(SolDivider());
			pathingTestingGroup.Add(SolLabel("Nav Beacons"));

			var navBeaconCol = SolCol();

			pathingTestingGroup.Add(navBeaconCol);

			var navBeaconList = SolList(
				controlPanel.NavBeacons,
				makeItem: () =>
				{
					var row = SolRow(true).WithStyle(r => r.marginLeft = 0);

					row.name = "row";

					var label = SolLabel();
					label.name = "label";
					row.Add(label);

					var button = SolButton();
					button.name = "btn";
					button.userData = -1;

					button.RegisterCallback<ClickEvent>(_ =>
						{
							if (button.userData is int i and >= 0)
								controlPanel.Pathing.NavMeshAgent.GoTo(controlPanel.NavBeacons[i].transform.position);
						}
					);

					row.Add(button);

					return row;
				},
				bindItem: (element, index) =>
				{
					element.schedule.Execute(() =>
						{
							var qLabel = element.Q("label");

							if (qLabel is Label label) label.text = controlPanel.NavBeacons[index].name;

							var qBtn = element.Q<Button>("btn");

							if (qBtn != null)
							{
								qBtn.text = "Go To";
								qBtn.userData = index;
								qBtn.SetEnabled(Application.isPlaying);
							}
						}
					);
				}
			);

			navBeaconList.showAddRemoveFooter = false;
			navBeaconList.reorderable = false;

			navBeaconCol.Add(navBeaconList);
		}

		private void GenerateCurrentStateType(ControlPanel controlPanel, VisualElement pathingTestingPanel)
		{
			if (controlPanel.Pathing == null) return;

			var pathing = new SerializedObject(controlPanel.Pathing);
			var stateNames = Enum.GetNames(typeof(StateType));

			string StateName(int idx)
			{
				return idx >= 0 && idx < stateNames.Length ? stateNames[idx] : "Undefined";
			}

			var currentStateProp = pathing.FindProperty("CurrentStateType");

			var currentStateTypeLabel = SolLabel("Current State Type: " + StateName(currentStateProp.enumValueIndex));

			currentStateTypeLabel.TrackPropertyValue(
				currentStateProp,
				p => currentStateTypeLabel.text = "Current State Type: " + StateName(p.enumValueIndex)
			);

			pathingTestingPanel.Add(SolDivider());

			AppendSolGrid(
					pathingTestingPanel,
					new VisualElement[]
					{
						currentStateTypeLabel,
					}
				)
			 .Bind(pathing);
		}

		private void GenerateHP(ControlPanel controlPanel, VisualElement flashlightTestingPanel)
		{
			if (controlPanel.Health == null) return;

			var health = new SerializedObject(controlPanel.Health);

			flashlightTestingPanel.Add(SolDivider());

			AppendSolGrid(
					flashlightTestingPanel,
					new VisualElement[]
					{
						SolFloatField(health.FindProperty("HP").FindPropertyRelative("_value"), false, "HP Value"),
						SolFloatField(health.FindProperty("HP").FindPropertyRelative("Max"), false, "HP Max"),
					}
				)
			 .Bind(health);
		}
	}
}