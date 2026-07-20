using System;
using Helpers.Attributes;
using UnityEngine.UIElements;

namespace Helpers.Editor.Ext
{
	[AiGenerated("Claude", "Sonnet 4.6")]
	public static class VisualElementExt
	{
		public static T WithChildren<T>(this T parent, params VisualElement[] children)
			where T : VisualElement
		{
			foreach (var child in children) parent.Add(child);

			// Usage
			// root.Add(SolarizedUI.Card().WithChildren(
			//         SolarizedUI.Label("Title", emphasized: true),
			//         SolarizedUI.Divider(),
			//         SolarizedUI.PrimaryButton("Go", () => { })
			//     ));

			return parent;
		}

		public static T WithClass<T>(this T el, params string[] classNames)
			where T : VisualElement
		{
			if (classNames == null) return el;

			foreach (var className in classNames) el.AddToClassList(className);

			return el;
		}

		public static T WithStyle<T>(this T el, params Action<IStyle>[] styleActions)
			where T : VisualElement
		{
			if (styleActions == null) return el;

			foreach (var styleAction in styleActions) styleAction(el.style);

			return el;
		}

		[AiGenerated("Gemini", "June 2026")]
		public static void SetReadonly(this Toggle toggle, bool readOnly)
		{
			if (readOnly)
			{
				// Register the trick callback using TrickleDown to catch the event early
				toggle.RegisterCallback<NavigationSubmitEvent>(BlockToggleEvent, TrickleDown.TrickleDown);
				toggle.RegisterCallback<ClickEvent>(BlockToggleEvent, TrickleDown.TrickleDown);
			}
			else
			{
				toggle.UnregisterCallback<NavigationSubmitEvent>(BlockToggleEvent, TrickleDown.TrickleDown);
				toggle.UnregisterCallback<ClickEvent>(BlockToggleEvent, TrickleDown.TrickleDown);
			}
		}

		[AiGenerated("Gemini", "June 2026")]
		private static void BlockToggleEvent(EventBase evt)
		{
			// Stop the click or spacebar submit event from propagating to the toggle's inner value-change logic
			evt.StopImmediatePropagation();
			evt.PreventDefault();
		}
	}
}