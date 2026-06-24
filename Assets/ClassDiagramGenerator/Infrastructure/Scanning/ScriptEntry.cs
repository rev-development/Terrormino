using System;
using System.IO;

namespace ClassDiagramGenerator.Infrastructure.Scanning
{
    [Serializable]
    public class ScriptEntry
    {
        public string Path;
        public bool IsSelected = true;

        public string FileName => System.IO.Path.GetFileName(Path);
        public string Directory => System.IO.Path.GetDirectoryName(Path)?.Replace('\\', '/');
    }
}