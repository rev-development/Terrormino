using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClassDiagramGenerator.Application.Abstractions;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Application
{
    public sealed class DiagramGenerationService
    {
        private readonly IClassDiagramParser _parser;
        private readonly IPlantUmlExporter _exporter;

        public DiagramGenerationService(IClassDiagramParser parser, IPlantUmlExporter exporter)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
        }

        public GenerateDiagramResult Generate(GenerateDiagramRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.SelectedScriptPaths == null || request.SelectedScriptPaths.Count == 0)
            {
                return new GenerateDiagramResult
                {
                    Success = false,
                    StatusMessage = "No scripts selected.",
                    Warnings = Array.Empty<string>()
                };
            }

            var types = new List<DiagramTypeModel>();
            var warnings = new List<string>();

            foreach (string scriptPath in request.SelectedScriptPaths)
            {
                if (string.IsNullOrWhiteSpace(scriptPath))
                    continue;

                if (!File.Exists(scriptPath))
                {
                    warnings.Add($"Missing file: {scriptPath}");
                    continue;
                }

                string content = File.ReadAllText(scriptPath, Encoding.UTF8);
                var parsedTypes = _parser.ParseFile(content);

                if (parsedTypes == null || parsedTypes.Count == 0)
                    warnings.Add($"No class/interface found in: {scriptPath}");
                else
                    types.AddRange(parsedTypes);
            }

            if (types.Count == 0)
            {
                return new GenerateDiagramResult
                {
                    Success = false,
                    StatusMessage = "No classes detected. Check the parser coverage.",
                    Warnings = warnings
                };
            }

            string plantUml = _exporter.Export(types, request.IncludeAssociations);

            return new GenerateDiagramResult
            {
                Success = true,
                StatusMessage = $"Diagram generated in memory. Types: {types.Count}",
                PlantUmlText = plantUml,
                TypeCount = types.Count,
                Warnings = warnings
            };
        }
    }
}