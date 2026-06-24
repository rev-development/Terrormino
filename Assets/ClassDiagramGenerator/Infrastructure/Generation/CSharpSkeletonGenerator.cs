using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassDiagramGenerator.Application;
using ClassDiagramGenerator.Application.Abstractions;
using ClassDiagramGenerator.Domain.Models;
using ClassDiagramGenerator.Infrastructure.Utilities;

namespace ClassDiagramGenerator.Infrastructure.Generation
{
    public sealed class CSharpSkeletonGenerator : ICodeSkeletonGenerator
    {
        public GenerateSkeletonResult Generate(GenerateSkeletonRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var createdFiles = new List<string>();
            var skippedFiles = new List<string>();
            var warnings = new List<string>();

            if (request.Types == null || request.Types.Count == 0)
            {
                return new GenerateSkeletonResult
                {
                    Success = false,
                    StatusMessage = "No imported types to generate.",
                    CreatedFiles = Array.Empty<string>(),
                    SkippedFiles = Array.Empty<string>(),
                    Warnings = Array.Empty<string>()
                };
            }

            if (string.IsNullOrWhiteSpace(request.OutputRootPath))
            {
                return new GenerateSkeletonResult
                {
                    Success = false,
                    StatusMessage = "Output root path is empty.",
                    CreatedFiles = Array.Empty<string>(),
                    SkippedFiles = Array.Empty<string>(),
                    Warnings = Array.Empty<string>()
                };
            }

            foreach (var type in request.Types)
            {
                if (type == null || string.IsNullOrWhiteSpace(type.Name))
                {
                    warnings.Add("Skipped one invalid type entry.");
                    continue;
                }

                string assetPath = BuildAssetPath(request.OutputRootPath, type.Name);
                string content = BuildFileContent(type);

                bool created = request.SkipExistingFiles
                    ? UnityAssetFileWriter.WriteTextAssetIfMissing(assetPath, content)
                    : WriteAlways(assetPath, content);

                if (created)
                    createdFiles.Add(assetPath);
                else
                    skippedFiles.Add(assetPath);
            }

            return new GenerateSkeletonResult
            {
                Success = createdFiles.Count > 0 || skippedFiles.Count > 0,
                StatusMessage = $"Skeleton generation done. Created: {createdFiles.Count}, Skipped: {skippedFiles.Count}",
                CreatedCount = createdFiles.Count,
                SkippedCount = skippedFiles.Count,
                CreatedFiles = createdFiles,
                SkippedFiles = skippedFiles,
                Warnings = warnings
            };
        }

        private static bool WriteAlways(string assetPath, string content)
        {
            UnityAssetFileWriter.WriteTextAsset(assetPath, content);
            return true;
        }

        private static string BuildAssetPath(string root, string typeName)
        {
            string cleanRoot = root.Replace('\\', '/').TrimEnd('/');
            return $"{cleanRoot}/{typeName}.cs";
        }

        private static string BuildFileContent(DiagramTypeModel type)
        {
            var sb = new StringBuilder();

            AppendUsings(sb, type);

            if (!string.IsNullOrWhiteSpace(type.Namespace))
            {
                sb.AppendLine($"namespace {type.Namespace}");
                sb.AppendLine("{");
                AppendTypeDeclaration(sb, type, 1);
                sb.AppendLine("}");
            }
            else
            {
                AppendTypeDeclaration(sb, type, 0);
            }

            return sb.ToString();
        }

        private static void AppendUsings(StringBuilder sb, DiagramTypeModel type)
        {
            var usings = new HashSet<string>();

            if (NeedsUnityEngine(type))
                usings.Add("using UnityEngine;");

            foreach (string item in usings.OrderBy(x => x, StringComparer.Ordinal))
                sb.AppendLine(item);

            if (usings.Count > 0)
                sb.AppendLine();
        }

        private static bool NeedsUnityEngine(DiagramTypeModel type)
        {
            bool MatchUnityType(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return false;

                string stripped = StripGenerics(value);
                return stripped == "MonoBehaviour"
                    || stripped == "ScriptableObject"
                    || stripped == "Vector2"
                    || stripped == "Vector3"
                    || stripped == "Vector4"
                    || stripped == "Quaternion"
                    || stripped == "GameObject"
                    || stripped == "Transform"
                    || stripped == "Color"
                    || stripped == "Sprite"
                    || stripped == "Texture2D";
            }

            if (MatchUnityType(type.BaseType))
                return true;

            if (type.Fields.Any(f => MatchUnityType(f.Type)))
                return true;

            if (type.Properties.Any(p => MatchUnityType(p.Type)))
                return true;

            if (type.Methods.Any(m =>
                    MatchUnityType(m.ReturnType) ||
                    m.Parameters.Any(p => MatchUnityType(p.Type))))
                return true;

            return false;
        }

        private static void AppendTypeDeclaration(StringBuilder sb, DiagramTypeModel type, int indentLevel)
        {
            string indent = Indent(indentLevel);
            string visibility = type.IsInterface ? "public" : "public";
            string abstractToken = type.IsAbstract && !type.IsInterface ? "abstract " : string.Empty;
            string kind = type.IsInterface ? "interface" : "class";
            string inheritance = BuildInheritance(type);

            if (!string.IsNullOrWhiteSpace(type.Summary))
            {
                sb.AppendLine($"{indent}/// <summary>");
                sb.AppendLine($"{indent}/// {type.Summary}");
                sb.AppendLine($"{indent}/// </summary>");
            }

            sb.AppendLine($"{indent}{visibility} {abstractToken}{kind} {type.Name}{inheritance}");
            sb.AppendLine($"{indent}{{");

            foreach (var field in type.Fields)
                sb.AppendLine($"{Indent(indentLevel + 1)}{MapVisibility(field.Visibility)} {field.Type} {field.Name};");

            if (type.Fields.Count > 0 && (type.Properties.Count > 0 || type.Methods.Count > 0))
                sb.AppendLine();

            foreach (var property in type.Properties)
            {
                sb.AppendLine(
                    $"{Indent(indentLevel + 1)}{MapVisibility(property.Visibility)} {property.Type} {property.Name} {{ get; set; }}");
            }

            if (type.Properties.Count > 0 && type.Methods.Count > 0)
                sb.AppendLine();

            foreach (var method in type.Methods)
            {
                string parameters = string.Join(", ",
                    method.Parameters.Select(p => $"{p.Type} {p.Name}"));

                if (type.IsInterface)
                {
                    sb.AppendLine(
                        $"{Indent(indentLevel + 1)}{method.ReturnType} {method.Name}({parameters});");
                    continue;
                }

                sb.AppendLine(
                    $"{Indent(indentLevel + 1)}{MapVisibility(method.Visibility)} {method.ReturnType} {method.Name}({parameters})");
                sb.AppendLine($"{Indent(indentLevel + 1)}{{");

                string returnStatement = BuildReturnStatement(method.ReturnType);
                if (!string.IsNullOrEmpty(returnStatement))
                    sb.AppendLine($"{Indent(indentLevel + 2)}{returnStatement}");

                sb.AppendLine($"{Indent(indentLevel + 1)}}}");
                sb.AppendLine();
            }

            sb.AppendLine($"{indent}}}");
        }

        private static string BuildInheritance(DiagramTypeModel type)
        {
            var items = new List<string>();

            if (!string.IsNullOrWhiteSpace(type.BaseType))
                items.Add(type.BaseType);

            if (type.Interfaces != null && type.Interfaces.Count > 0)
                items.AddRange(type.Interfaces.Where(i => !string.IsNullOrWhiteSpace(i)));

            return items.Count > 0 ? " : " + string.Join(", ", items) : string.Empty;
        }

        private static string BuildReturnStatement(string returnType)
        {
            if (string.IsNullOrWhiteSpace(returnType) || returnType == "void")
                return string.Empty;

            return "return default;";
        }

        private static string MapVisibility(string symbol)
        {
            return symbol switch
            {
                "+" => "public",
                "-" => "private",
                "#" => "protected",
                "~" => "internal",
                _ => "private"
            };
        }

        private static string StripGenerics(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return typeName;

            int index = typeName.IndexOf('<');
            return index >= 0 ? typeName.Substring(0, index) : typeName;
        }

        private static string Indent(int level)
        {
            return new string(' ', level * 4);
        }
    }
}