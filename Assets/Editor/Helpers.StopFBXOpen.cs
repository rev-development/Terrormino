using UnityEditor;
using UnityEditor.Callbacks;

public class StopFBXOpen
{

    // The OnOpenAsset attribute intercepts double-clicks on any asset in the project
    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        // Get the path of the asset being double-clicked
        string assetPath = AssetDatabase.GetAssetPath(instanceID);

        // Check if the file extension is .fbx (case-insensitive)
        if (assetPath.ToLower().EndsWith(".fbx"))
        {
            // Return true to tell Unity: "We handled this asset, do not pass it to the OS"
            return true;
        }

        // Return false for all other assets (C# scripts, scenes, etc.) so they open normally
        return false;
    }

}