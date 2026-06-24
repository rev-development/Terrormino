using System;
using System.IO;
using System.Text;
using UnityEditor;

namespace ClassDiagramGenerator.Infrastructure.Utilities
{
    public static class UnityAssetFileWriter
    {
        public static void WriteTextAsset(string assetPath, string content)
        {
            EnsureOutputDirectoryExists(assetPath);
            File.WriteAllText(assetPath, content, new UTF8Encoding(false));
            AssetDatabase.Refresh();
        }
        
        public static bool WriteTextAssetIfMissing(string assetPath, string content)
        {
            EnsureOutputDirectoryExists(assetPath);

            if (File.Exists(assetPath))
                return false;

            File.WriteAllText(assetPath, content, new UTF8Encoding(false));
            AssetDatabase.Refresh();
            return true;
        }

        private static void EnsureOutputDirectoryExists(string assetFilePath)
        {
            if (string.IsNullOrWhiteSpace(assetFilePath))
                throw new InvalidOperationException("Output path is empty.");

            if (!assetFilePath.StartsWith("Assets/", StringComparison.Ordinal) && assetFilePath != "Assets")
                throw new InvalidOperationException("Output path must stay inside the Unity project under 'Assets/'.");

            string folder = Path.GetDirectoryName(assetFilePath)?.Replace('\\', '/');
            if (string.IsNullOrWhiteSpace(folder))
                throw new InvalidOperationException("Invalid output folder.");

            CreateAssetFolderRecursive(folder);
        }

        private static void CreateAssetFolderRecursive(string assetFolder)
        {
            if (AssetDatabase.IsValidFolder(assetFolder))
                return;

            string[] parts = assetFolder.Split('/');
            if (parts.Length == 0 || parts[0] != "Assets")
                throw new InvalidOperationException($"Invalid Unity asset folder: {assetFolder}");

            string current = "Assets";
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[index]);

                current = next;
            }
        }
    }
}