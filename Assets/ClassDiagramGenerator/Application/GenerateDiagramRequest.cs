using System.Collections.Generic;

namespace ClassDiagramGenerator.Application
{
    public sealed class GenerateDiagramRequest
    {
        public IReadOnlyList<string> SelectedScriptPaths { get; set; }
        public bool IncludeAssociations { get; set; }
    }
}