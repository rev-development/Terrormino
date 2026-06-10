using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.UIElements;

namespace AYellowpaper.SerializedCollections.Editor
{
    public class EditorUserSettingsProvider : SettingsProvider
    {

        public const string PreferencesPath = "Preferences/Serialized Collections";

        private SerializedProperty _alwaysShowSearch;

        private SerializedProperty _elementsPerPage;

        private SerializedProperty _pageCountForSearch;

        private AnimBool _searchAnimBool;

        private SerializedObject _serializedObject;

        public EditorUserSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new EditorUserSettingsProvider(PreferencesPath);

            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();

            return provider;
        }

        public static bool IsSettingsAvailable()
        {
            return EditorUserSettings.Get() != null;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            EnsureSerializedObjectExists();
        }

        private void EnsureSerializedObjectExists()
        {
            if (_serializedObject == null)
            {
                _searchAnimBool = new AnimBool();
                _searchAnimBool.valueChanged.AddListener(Repaint);
                _serializedObject = new SerializedObject(EditorUserSettings.Get());
                _alwaysShowSearch = _serializedObject.FindProperty("_alwaysShowSearch");
                _pageCountForSearch = _serializedObject.FindProperty("_pageCountForSearch");
                _elementsPerPage = _serializedObject.FindProperty("_elementsPerPage");
            }
        }

        public override void OnGUI(string searchContext)
        {
            EnsureSerializedObjectExists();

            EditorGUI.indentLevel = 1;

            _serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(_alwaysShowSearch);
            _searchAnimBool.target = !_alwaysShowSearch.boolValue;

            using (var group = new EditorGUILayout.FadeGroupScope(_searchAnimBool.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.PropertyField(_pageCountForSearch);
                }
            }

            EditorGUILayout.PropertyField(_elementsPerPage);

            bool changed = _serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorUserSettings.Save();
            }
        }

        private class Styles
        {

        }

    }
}