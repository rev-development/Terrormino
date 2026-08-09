using System;
using System.Collections.Generic;
using EC.Tetris;
using Helpers.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Helpers.Editor.Theming.SolarizedDark.Ele;

namespace Editor.ControlPanels
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ControlPanel))]
	public class Tetris : ControlPanelDrawerBase<ControlPanel>
	{
		protected override List<Func<ControlPanel, VisualElement>> _customPanelGenerationFunctions =>
			new()
			{
				GenerateTetrisControllerPanel,
			};

		private VisualElement GenerateTetrisControllerPanel(ControlPanel controlPanel)
		{
			var panel = SolGrid(
				"TetrisController Testing",
				new VisualElement[]
				{
					SolButton(_ => controlPanel.Controller.StartGame(), "Start Tetris Game", Application.isPlaying),
				}
			);

			return panel;
		}
	}
}