using System.Collections.Generic;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Application
{
    public sealed class GenerateSkeletonRequest
    {
        public IReadOnlyList<DiagramTypeModel> Types { get; set; }
        public string OutputRootPath { get; set; }
        public bool SkipExistingFiles { get; set; } = true;
    }
}