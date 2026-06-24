using System.Collections.Generic;

namespace ClassDiagramGenerator.Domain.Models
{
    public sealed class DiagramMethodModel
    {
        public string Name;
        public string ReturnType;
        public string Visibility;
        public List<DiagramParameterModel> Parameters = new();
    }
}