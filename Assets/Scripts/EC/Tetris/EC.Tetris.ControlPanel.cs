using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace EC.Tetris
{
	[DisallowMultipleComponent]
	[AddComponentMenu("EC.Tetris.ControlPanel")]
	public class ControlPanel : ControlPanelBase
	{
		[HideInInspector] public EventBus EventBus;

		[HideInInspector] public Controller Controller;

		[HideInInspector] public InputAdapter InputAdapter;

		[HideInInspector] public PlayfieldRenderer PlayfieldRenderer;

		[HideInInspector] public MKBControls MKBControls;

		protected override List<MonoBehaviour> GetComponents()
		{
			EventBus = gameObject.GetComponent<EventBus>();
			Controller = gameObject.GetComponent<Controller>();
			InputAdapter = gameObject.GetComponent<InputAdapter>();
			PlayfieldRenderer = gameObject.GetComponent<PlayfieldRenderer>();
			MKBControls = gameObject.GetComponent<MKBControls>();

			return new List<MonoBehaviour>
			{
				EventBus,
				Controller,
				InputAdapter,
				PlayfieldRenderer,
				MKBControls,
			};
		}
	}
}