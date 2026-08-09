using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Attributes;
using Helpers.Editor.Ext;
using Helpers.Editor.Theming.SolarizedDark;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Helpers.Editor.Theming.SolarizedDark.Ele;
using Object = UnityEngine.Object;

namespace Helpers.Editor
{
	[PublicAPI]
	[CanEditMultipleObjects]
	public abstract class ControlPanelDrawerBase<TControlPanelMono> : UnityEditor.Editor
		where TControlPanelMono : ControlPanelBase
	{
		protected abstract List<Func<TControlPanelMono, VisualElement>> _customPanelGenerationFunctions { get; }

		[AiGenerated("Claude", "Sonnet 4.6")]
		protected Foldout GenerateComponentFoldout(SerializedObject so, GameObject mainGO, List<SerializedObject> subSOs)
		{
			var type = so.targetObject.GetType();

			// Build a HashSet so we can check membership in one step instead of scanning the whole list each time
			// Big O Notation: O(N) to build once, O(1) per Contains — "O" describes how work scales as collection size grows
			var subTargets = new HashSet<Object>(subSOs.Select(s => s.targetObject));

			var foldout = SolFoldout(type.FullName);
			foldout.name = type.FullName;
			foldout.viewDataKey = $"{mainGO.GetInstanceID()}_{type.Name}_Foldout";

			so.IterateProps(
				foldout,
				prop => prop.name == "m_Script"
								|| (prop.propertyType == SerializedPropertyType.ObjectReference
										&& (subTargets.Contains(prop.objectReferenceValue) || prop.objectReferenceValue == target)),
				new[]
				{
					StyleHelper.VField,
				}
			);

			return foldout;
		}

		protected VisualElement GenerateConfigPanel(TControlPanelMono controlPanel) =>
			SolGrid(
				"Config",
				new VisualElement[]
				{
					SolButton(_ => controlPanel.GetInitializedComponents(), "Initialize Components", !Application.isPlaying),
				}
			);

		public override VisualElement CreateInspectorGUI()
		{
			// 1. Setup Base Element
			var root = SolRoot();

			// 2. Get a reference to the actual ControlPanel MonoBehaviour we're referencing
			var controlPanel = (TControlPanelMono)target;

			// 3. Get the initialized components from the ControlPanel
			var components = controlPanel.GetInitializedComponents()
																	 .Select(component => new SerializedObject(component))
																	 .ToList();

			// 4. _customPanelGenerationFunctions is where anything tailored to a specific type of ControlPanel should go 
			if (_customPanelGenerationFunctions != null)
				_customPanelGenerationFunctions.ForEach(panelFunc => root.Add(panelFunc(controlPanel)));

			// 4a. Add ConfigPanel at the end
			root.Add(GenerateConfigPanel(controlPanel));

			// 5. Make a SerializedObject for this panel so data can be bound to it.
			var controlPanelSO = new SerializedObject(controlPanel);

			// 6. Generate the regular fields
			InspectorElement.FillDefaultInspector(root, controlPanelSO, this);

			// 7. Generate foldouts to contain the default inspectors of the related components (this is a QoL thing so you don't have to root around in child objects).
			root.Add(SolDivider());
			root.Add(SolLabel("Related Components"));

			components.Select(so => GenerateComponentFoldout(so, controlPanel.gameObject, components))
								.ToList()
								.ForEach(root.Add);

			return root;
		}
	}
}