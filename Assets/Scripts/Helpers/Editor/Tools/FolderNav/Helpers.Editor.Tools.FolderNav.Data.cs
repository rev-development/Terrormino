// Editor/Tools/FolderNavigatorData.cs

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Helpers.Editor.Tools.FolderNav
{
	[Serializable]
	public class FolderEntry
	{
		public string Label;

		public string Path;
	}

	[Serializable]
	public class FolderEntryList
	{
		public List<FolderEntry> Entries = new();
	}

	public static class FolderNavigatorData
	{
		private const string PrefsKey = "MyStudio.FolderNavigator.Folders";

		public static void Reset() => EditorPrefs.DeleteKey(PrefsKey);

		public static List<FolderEntry> Load()
		{
			var json = EditorPrefs.GetString(PrefsKey, null);

			if (string.IsNullOrEmpty(json)) return Defaults();

			return JsonUtility.FromJson<FolderEntryList>(json)?.Entries ?? Defaults();
		}

		public static void Save(List<FolderEntry> entries)
		{
			var json = JsonUtility.ToJson(
				new FolderEntryList
				{
					Entries = entries,
				}
			);

			EditorPrefs.SetString(PrefsKey, json);
		}

		private static List<FolderEntry> Defaults() =>
			new()
			{
				new FolderEntry
				{
					Label = "Scripts",
					Path = "Assets/Scripts",
				},
				new FolderEntry
				{
					Label = "Editor",
					Path = "Assets/Editor",
				},
				new FolderEntry
				{
					Label = "Prefabs",
					Path = "Assets/Prefabs",
				},
				new FolderEntry
				{
					Label = "Scenes",
					Path = "Assets/Scenes",
				},
				new FolderEntry
				{
					Label = "ScriptableObjects",
					Path = "Assets/ScriptableObjects",
				},
				new FolderEntry
				{
					Label = "Animations",
					Path = "Assets/Animations",
				},
				new FolderEntry
				{
					Label = "Audio",
					Path = "Assets/Audio",
				},
				new FolderEntry
				{
					Label = "Controls",
					Path = "Assets/Controls",
				},

				new FolderEntry
				{
					Label = "Models",
					Path = "Assets/Models",
				},

				new FolderEntry
				{
					Label = "Shaders",
					Path = "Assets/Shaders",
				},
			};
	}
}