using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.Theming
{
	[PublicAPI]
	public class UtilityClasses
	{
		public static List<UssClass> Text = new()
		{
			new UssClass("font-weight-bold", "-unity-font-style", "bold"),
			new UssClass("font-weight-normal", "-unity-font-style", "normal"),
			new UssClass("font-color-default", "color", $"{Helpers.Editor.Theming.OriginalUnity.Palette.DefaultText}"),
			new UssClass("font-size-md", "font-size", "11px"),
		};

		public static List<UssClass> Flex = new()
		{
			new UssClass("flex-row", "flex-direction", "row"),
			new UssClass("flex-column", "flex-direction", "column"),
			new UssClass("flex-grow-1", "flex-grow", "1"),
			new UssClass("flex-grow-0", "flex-grow", "0"),
			new UssClass("flex-shrink-1", "flex-shrink", "1"),
			new UssClass("flex-shrink-0", "flex-shrink", "0"),
		};

		public List<UssClass> GenerateSpacingHelpers(int increments)
		{
			var spacingHelpers = new List<UssClass>();

			foreach (var cssPropName in new[]
					 {
						 "padding",
						 "margin",
					 })
			{
				for (var i = 1; i <= increments; i++)
					spacingHelpers.Add(new UssClass(cssPropName[..1] + $"a{i}", cssPropName, $"{i * 4}px"));
			}

			return spacingHelpers;
		}

		public void GenerateUSSFile(string ussPath)
		{
			var path = ussPath;

			if (path == null) return;

			var directory = Path.GetDirectoryName(path);

			if (!Directory.Exists(directory)
				&& directory != null)
				Directory.CreateDirectory(directory);

			File.WriteAllText(path, BuildUSS());

			AssetDatabase.ImportAsset(path);
			AssetDatabase.Refresh();

			Debug.Log($"USS written to {path}");
		}

		public string BuildUSS()
		{
			var utilityClasses = new List<UssClass>();

			utilityClasses.AddRange(Text);
			utilityClasses.AddRange(Flex);
			utilityClasses.AddRange(GenerateSpacingHelpers(16));

			return string.Join("\n\n", utilityClasses.Select(utilityClass => utilityClass.ToString()));
		}

		public StyleSheet GetStyleSheet(StyleSheet cachedSheet, string ussPath, Dictionary<string, UssProps> ussDict)
		{
			if (cachedSheet != null) return cachedSheet;

			cachedSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);

			if (cachedSheet == null)
			{
				GenerateUSSFile(ussPath);
				cachedSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
			}

			return cachedSheet;
		}

		public static string GetUssPath(string themeName)
		{
			var guids = AssetDatabase.FindAssets($"{typeof(UtilityClasses).FullName} t:Script");

			if (guids.Length == 0)
			{
				Debug.LogWarning(
					$"{typeof(UtilityClasses).Namespace?.Split('.')[^1]}.{nameof(UtilityClasses)} script asset not found — USS path could not be resolved."
				);

				return null;
			}

			var scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
			var themeDir = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath));

			return (themeDir + "/UtilityClasses.uss").Replace('\\', '/');
		}

		public void ApplyTo(
			VisualElement root,
			StyleSheet cachedSheet,
			string ussPath,
			Dictionary<string, UssProps> ussDict
		) =>
			root.styleSheets.Add(GetStyleSheet(cachedSheet, ussPath, ussDict));

		public class UssClass
		{
			[PublicAPI] public string Name;

			[PublicAPI] public UssProps Props = new();

			public UssClass(string name, Dictionary<string, string> props)
			{
				Name = name;

				foreach (var keyValuePair in props) Props.Set(keyValuePair.Key, keyValuePair.Value);
			}

			public UssClass(string name, string key, string value) // Shortcut for single property classes
			{
				Name = name;

				Props.Set(key, value);
			}

			public new string ToString() => Props.ToUss(Name);
		}
	}
}