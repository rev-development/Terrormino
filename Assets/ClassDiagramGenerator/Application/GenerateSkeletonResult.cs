using System.Collections.Generic;

namespace ClassDiagramGenerator.Application
{
    public sealed class GenerateSkeletonResult
    {
        public bool Success { get; set; }
        public string StatusMessage { get; set; }
        public int CreatedCount { get; set; }
        public int SkippedCount { get; set; }
        public IReadOnlyList<string> CreatedFiles { get; set; }
        public IReadOnlyList<string> SkippedFiles { get; set; }
        public IReadOnlyList<string> Warnings { get; set; }
    }
}