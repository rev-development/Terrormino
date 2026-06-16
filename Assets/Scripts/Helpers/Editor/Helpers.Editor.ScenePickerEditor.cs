using UnityEditor;

[CustomEditor(typeof(Helpers.ScenePicker), true)]
public class ScenePickerEditor : UnityEditor.Editor
{

    public override void OnInspectorGUI()
    {
        var picker = target as Helpers.ScenePicker;
        var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.ScenePath);

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        var newScene = EditorGUILayout.ObjectField(
                "scene",
                oldScene,
                typeof(SceneAsset),
                false
            ) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            string newPath = AssetDatabase.GetAssetPath(newScene);
            var scenePathProperty = serializedObject.FindProperty("ScenePath");
            scenePathProperty.stringValue = newPath;
        }

        serializedObject.ApplyModifiedProperties();
    }

}