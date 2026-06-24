using System.Collections.Generic;

namespace ClassDiagramGenerator.Domain.Models
{
    public sealed class DiagramTypeModel
    {
        public string Name;
        public string Namespace;
        public string Summary;
        public string BaseType;
        public bool IsAbstract;
        public bool IsInterface;
        public List<string> Interfaces = new();
        public List<DiagramFieldModel> Fields = new();
        public List<DiagramPropertyModel> Properties = new();
        public List<DiagramMethodModel> Methods = new();
        public List<string> LocalCalls = new();
    }
}