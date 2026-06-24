using System.Collections.Generic;

namespace ClassDiagramGenerator.Application
{
    public sealed class GenerateDiagramResult
    {
        public bool Success { get; set; }
        public string StatusMessage { get; set; }
        public string PlantUmlText { get; set; }
        public int TypeCount { get; set; }
        public IReadOnlyList<string> Warnings { get; set; }
    }
}