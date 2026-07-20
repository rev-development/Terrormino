using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helpers.Editor
{
	[Helpers.Attributes.AiGeneratedAttribute("Claude", "Sonnet 4.6")]
	public class UssProps
	{
		private readonly Dictionary<string, string> _props = new();

		public UssProps Set(string property, string value)
		{
			if (_props.ContainsKey(property)) Debug.LogWarning($"UssProps: '{property}' set twice — overwriting.");

			_props[property] = value;

			return this;
		}

		// Returns a new UssProps — base mixins stay reusable
		public UssProps Merge(params UssProps[] others)
		{
			var result = new UssProps();
			foreach (var kvp in _props) result._props[kvp.Key] = kvp.Value;

			foreach (var other in others)
			{
				foreach (var kvp in other._props) result._props[kvp.Key] = kvp.Value;
			}

			return result;
		}

		public string ToUss() => string.Join("\n    ", _props.Select(kvp => $"{kvp.Key}: {kvp.Value};"));

		public string ToUss(string className) =>
			$".{className} {{\n    {string.Join("\n    ", _props.Select(kvp => $"{kvp.Key}: {kvp.Value};"))}\n}}";

		public static UssProps FromUssClass(string ussClass)
		{
			var result = new UssProps();
			var start = ussClass.IndexOf('{') + 1;
			var body = ussClass.Substring(start, ussClass.LastIndexOf('}') - start);

			foreach (var line in body.Split(';'))
			{
				var trimmed = line.Trim();

				if (string.IsNullOrEmpty(trimmed)) continue;

				var colon = trimmed.IndexOf(':');

				if (colon < 0) continue;

				result.Set(trimmed.Substring(0, colon).Trim(), trimmed.Substring(colon + 1).Trim());
			}

			return result;
		}
	}
}