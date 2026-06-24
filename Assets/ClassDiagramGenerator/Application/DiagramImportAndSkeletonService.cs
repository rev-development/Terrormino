using System;
using ClassDiagramGenerator.Application.Abstractions;

namespace ClassDiagramGenerator.Application
{
    public sealed class DiagramImportAndSkeletonService
    {
        private readonly IClassDiagramImporter _importer;
        private readonly ICodeSkeletonGenerator _generator;

        public DiagramImportAndSkeletonService(
            IClassDiagramImporter importer,
            ICodeSkeletonGenerator generator)
        {
            _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public GenerateSkeletonResult ImportAndGenerate(string diagramText, string outputRootPath, bool skipExistingFiles = true)
        {
            if (string.IsNullOrWhiteSpace(diagramText))
            {
                return new GenerateSkeletonResult
                {
                    Success = false,
                    StatusMessage = "Diagram text is empty.",
                    CreatedFiles = Array.Empty<string>(),
                    SkippedFiles = Array.Empty<string>(),
                    Warnings = Array.Empty<string>()
                };
            }

            var types = _importer.Import(diagramText);

            return _generator.Generate(new GenerateSkeletonRequest
            {
                Types = types,
                OutputRootPath = outputRootPath,
                SkipExistingFiles = skipExistingFiles
            });
        }
    }
}