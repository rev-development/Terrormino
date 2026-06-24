using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ClassDiagramGenerator.Infrastructure.Scanning
{
    public sealed class ScriptSelectionManager
    {
        public List<ScriptEntry> Scripts { get; private set; } = new();

        public void Clear()
        {
            Scripts.Clear();
        }

        public void Scan(string assetFolderPath)
        {
            Scripts.Clear();

            if (string.IsNullOrWhiteSpace(assetFolderPath))
                return;

            if (!AssetDatabase.IsValidFolder(assetFolderPath))
                return;

            string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { assetFolderPath });

            Scripts = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(path => new ScriptEntry
                {
                    Path = Normalize(path),
                    IsSelected = true
                })
                .OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public void AddFiles(IEnumerable<string> paths)
        {
            if (paths == null)
                return;

            var existing = new HashSet<string>(
                Scripts.Select(s => Normalize(s.Path)),
                StringComparer.OrdinalIgnoreCase);

            foreach (string raw in paths)
            {
                string normalized = Normalize(raw);

                if (string.IsNullOrWhiteSpace(normalized))
                    continue;

                if (!normalized.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (existing.Contains(normalized))
                    continue;

                Scripts.Add(new ScriptEntry
                {
                    Path = normalized,
                    IsSelected = true
                });

                existing.Add(normalized);
            }

            Scripts = Scripts
                .OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public List<ScriptEntry> GetSelected()
        {
            return Scripts.Where(s => s.IsSelected).ToList();
        }

        private static string Normalize(string path)
        {
            return string.IsNullOrWhiteSpace(path)
                ? string.Empty
                : path.Replace('\\', '/');
        }
    }
}