using UnityEditor;

internal static class EditorMenus
{

    [MenuItem("Tools/Toggle Inspector Lock %l")]
    private static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

}