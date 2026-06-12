using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class StopFBXOpen
{

    // The OnOpenAsset attribute intercepts double-clicks on any asset in the project
    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line) {
        // Get the path of the asset being double-clicked
        var assetPath = AssetDatabase.GetAssetPath(instanceID);

        // Check if the file extension is .fbx (case-insensitive)
        if (assetPath.ToLower().EndsWith(".fbx"))
        {
            Debug.Log(
                    "The opening of a .fbx file inside Unity was prevented by Rev's Helpers.StopFBXOpen script inside the Editor folder.\n"
                    + "If you wish to open the file, you can open it normally using the File Explorer, this script only affects the double click behavior inside Unity."
                );

            Debug.Log(
                    "If you would like to always be able to open .fbx files from inside Unity, then delete this file.\n"
                    + "Rev keeps a separate repository for Helpers and this file is ignored by git when it is inside the Editor folder."
                );

            // Return true to tell Unity: "We handled this asset, do not pass it to the OS"
            return true;
        }

        // Return false for all other assets (C# scripts, scenes, etc.) so they open normally
        return false;
    }

}