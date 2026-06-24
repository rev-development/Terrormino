using System.Collections.Generic;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Application.Abstractions
{
    public interface IPlantUmlExporter
    {
        string Export(IReadOnlyList<DiagramTypeModel> types, bool includeAssociations);
    }
}