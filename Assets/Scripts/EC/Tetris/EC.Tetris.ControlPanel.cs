using System.Collections.Generic;
using Helpers.Editor;
using UnityEngine;

namespace EC.Tetris
{
	[DisallowMultipleComponent]
	public class ControlPanel : ControlPanelBase
	{
		[HideInInspector] public EventBus EventBus;

		[HideInInspector] public Controller Controller;

		[HideInInspector] public InputAdapter InputAdapter;

		[HideInInspector] public PlayfieldRenderer PlayfieldRenderer;

		protected override List<MonoBehaviour> GetComponents()
		{
			EventBus = gameObject.GetComponent<EventBus>();
			Controller = gameObject.GetComponent<Controller>();
			InputAdapter = gameObject.GetComponent<InputAdapter>();
			PlayfieldRenderer = gameObject.GetComponent<PlayfieldRenderer>();

			return new List<MonoBehaviour>
			{
				EventBus,
				Controller,
				InputAdapter,
				PlayfieldRenderer,
			};
		}
	}
}