using System.Collections.Generic;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Application.Abstractions
{
    public interface IClassDiagramParser
    {
        IReadOnlyList<DiagramTypeModel> ParseFile(string sourceText);
    }
}