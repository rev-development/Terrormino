using System.Collections.Generic;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Application.Abstractions
{
    public interface IClassDiagramImporter
    {
        IReadOnlyList<DiagramTypeModel> Import(string diagramText);
    }
}