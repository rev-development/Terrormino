// Editor/Tools/FolderNavigatorData.cs

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Helpers.Editor.Tools.FolderNav
{
    [System.Serializable]
    public class FolderEntry
    {

        public string Label;

        public string Path;

    }

    [System.Serializable]
    public class FolderEntryList
    {

        public List<FolderEntry> Entries = new();

    }

    public static class FolderNavigatorData
    {

        private const string PrefsKey = "MyStudio.FolderNavigator.Folders";

        public static List<FolderEntry> Load() {
            var json = EditorPrefs.GetString(PrefsKey, null);

            if (string.IsNullOrEmpty(json)) return Defaults();

            return JsonUtility.FromJson<FolderEntryList>(json)?.Entries ?? Defaults();
        }

        public static void Save(List<FolderEntry> entries) {
            var json = JsonUtility.ToJson(
                    new FolderEntryList
                    {
                        Entries = entries
                    }
                );

            EditorPrefs.SetString(PrefsKey, json);
        }

        public static void Reset() {
            EditorPrefs.DeleteKey(PrefsKey);
        }

        private static List<FolderEntry> Defaults() {
            return new List<FolderEntry>
                   {
                       new()
                       {
                           Label = "Scripts",
                           Path = "Assets/Scripts"
                       },
                       new()
                       {
                           Label = "Editor",
                           Path = "Assets/Editor"
                       },
                       new()
                       {
                           Label = "Prefabs",
                           Path = "Assets/Prefabs"
                       },
                       new()
                       {
                           Label = "Scenes",
                           Path = "Assets/Scenes"
                       },
                       new()
                       {
                           Label = "ScriptableObjects",
                           Path = "Assets/ScriptableObjects"
                       },
                       new()
                       {
                           Label = "Animations",
                           Path = "Assets/Animations"
                       },
                       new()
                       {
                           Label = "Audio",
                           Path = "Assets/Audio"
                       },
                       new()
                       {
                           Label = "Controls",
                           Path = "Assets/Controls"
                       },

                       new()
                       {
                           Label = "Models",
                           Path = "Assets/Models"
                       },

                       new()
                       {
                           Label = "Shaders",
                           Path = "Assets/Shaders"
                       }
                   };
        }

    }
}