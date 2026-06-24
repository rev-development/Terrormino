using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ClassDiagramGenerator.Application.Abstractions;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Infrastructure.Import
{
    public sealed class PlantUmlDiagramImporter : IClassDiagramImporter
    {
        private static readonly Regex TypeStartRegex = new(
            @"^(?<kind>class|interface)\s+(?<name>[A-Za-z_]\w*)(?<stereotype>\s+<<abstract>>)?\s*\{$",
            RegexOptions.Compiled);

        private static readonly Regex PropertyRegex = new(
            @"^(?<visibility>[+\-#~])\s+(?<name>[A-Za-z_]\w*)\s*:\s*(?<type>.+?)\s*\{\s*get;\s*set;\s*\}$",
            RegexOptions.Compiled);

        private static readonly Regex MethodRegex = new(
            @"^(?<visibility>[+\-#~])\s+(?<name>[A-Za-z_]\w*)\((?<params>.*)\)\s*:\s*(?<returnType>.+)$",
            RegexOptions.Compiled);

        private static readonly Regex FieldRegex = new(
            @"^(?<visibility>[+\-#~])\s+(?<name>[A-Za-z_]\w*)\s*:\s*(?<type>.+)$",
            RegexOptions.Compiled);

        private static readonly Regex InheritanceRegex = new(
            @"^(?<base>[A-Za-z_][\w<>]*)\s+<\|--\s+(?<child>[A-Za-z_]\w*)$",
            RegexOptions.Compiled);

        private static readonly Regex InterfaceRegex = new(
            @"^(?<iface>[A-Za-z_][\w<>]*)\s+<\|\.\.\s+(?<child>[A-Za-z_]\w*)$",
            RegexOptions.Compiled);

        private static readonly Regex SummaryRegex = new(
            @"^'\s*(?<name>[A-Za-z_]\w*)\s*:\s*(?<summary>.+)$",
            RegexOptions.Compiled);

        public IReadOnlyList<DiagramTypeModel> Import(string diagramText)
        {
            var result = new List<DiagramTypeModel>();
            if (string.IsNullOrWhiteSpace(diagramText))
                return result;

            var byName = new Dictionary<string, DiagramTypeModel>(StringComparer.OrdinalIgnoreCase);
            DiagramTypeModel currentType = null;

            string[] lines = diagramText
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line == "@startuml" || line == "@enduml")
                    continue;

                var typeStartMatch = TypeStartRegex.Match(line);
                if (typeStartMatch.Success)
                {
                    currentType = new DiagramTypeModel
                    {
                        Name = typeStartMatch.Groups["name"].Value,
                        IsInterface = typeStartMatch.Groups["kind"].Value == "interface",
                        IsAbstract = typeStartMatch.Groups["stereotype"].Success
                    };

                    result.Add(currentType);
                    byName[currentType.Name] = currentType;
                    continue;
                }

                if (line == "}")
                {
                    currentType = null;
                    continue;
                }

                var summaryMatch = SummaryRegex.Match(line);
                if (summaryMatch.Success)
                {
                    string name = summaryMatch.Groups["name"].Value;
                    if (byName.TryGetValue(name, out var type))
                        type.Summary = summaryMatch.Groups["summary"].Value.Trim();

                    continue;
                }

                var inheritanceMatch = InheritanceRegex.Match(line);
                if (inheritanceMatch.Success)
                {
                    string childName = inheritanceMatch.Groups["child"].Value;
                    if (byName.TryGetValue(childName, out var type))
                        type.BaseType = inheritanceMatch.Groups["base"].Value.Trim();

                    continue;
                }

                var interfaceMatch = InterfaceRegex.Match(line);
                if (interfaceMatch.Success)
                {
                    string childName = interfaceMatch.Groups["child"].Value;
                    if (byName.TryGetValue(childName, out var type))
                        type.Interfaces.Add(interfaceMatch.Groups["iface"].Value.Trim());

                    continue;
                }

                if (currentType == null)
                    continue;

                var propertyMatch = PropertyRegex.Match(line);
                if (propertyMatch.Success)
                {
                    currentType.Properties.Add(new DiagramPropertyModel
                    {
                        Visibility = propertyMatch.Groups["visibility"].Value,
                        Name = propertyMatch.Groups["name"].Value,
                        Type = propertyMatch.Groups["type"].Value.Trim()
                    });
                    continue;
                }

                var methodMatch = MethodRegex.Match(line);
                if (methodMatch.Success)
                {
                    currentType.Methods.Add(new DiagramMethodModel
                    {
                        Visibility = methodMatch.Groups["visibility"].Value,
                        Name = methodMatch.Groups["name"].Value,
                        ReturnType = methodMatch.Groups["returnType"].Value.Trim(),
                        Parameters = ParseParameters(methodMatch.Groups["params"].Value)
                    });
                    continue;
                }

                var fieldMatch = FieldRegex.Match(line);
                if (fieldMatch.Success)
                {
                    currentType.Fields.Add(new DiagramFieldModel
                    {
                        Visibility = fieldMatch.Groups["visibility"].Value,
                        Name = fieldMatch.Groups["name"].Value,
                        Type = fieldMatch.Groups["type"].Value.Trim()
                    });
                }
            }

            return result;
        }

        private static List<DiagramParameterModel> ParseParameters(string raw)
        {
            var result = new List<DiagramParameterModel>();
            if (string.IsNullOrWhiteSpace(raw))
                return result;

            foreach (string chunk in SplitParameters(raw))
            {
                string part = chunk.Trim();
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                int separatorIndex = part.IndexOf(':');
                if (separatorIndex <= 0 || separatorIndex >= part.Length - 1)
                    continue;

                string name = part.Substring(0, separatorIndex).Trim();
                string type = part.Substring(separatorIndex + 1).Trim();

                result.Add(new DiagramParameterModel
                {
                    Name = name,
                    Type = type
                });
            }

            return result;
        }

        private static List<string> SplitParameters(string raw)
        {
            var result = new List<string>();
            var sb = new System.Text.StringBuilder();
            int genericDepth = 0;
            int parenthesisDepth = 0;
            int bracketDepth = 0;

            foreach (char c in raw)
            {
                switch (c)
                {
                    case '<': genericDepth++; sb.Append(c); break;
                    case '>': genericDepth = Math.Max(0, genericDepth - 1); sb.Append(c); break;
                    case '(': parenthesisDepth++; sb.Append(c); break;
                    case ')': parenthesisDepth = Math.Max(0, parenthesisDepth - 1); sb.Append(c); break;
                    case '[': bracketDepth++; sb.Append(c); break;
                    case ']': bracketDepth = Math.Max(0, bracketDepth - 1); sb.Append(c); break;
                    case ',':
                        if (genericDepth == 0 && parenthesisDepth == 0 && bracketDepth == 0)
                        {
                            result.Add(sb.ToString());
                            sb.Clear();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            if (sb.Length > 0)
                result.Add(sb.ToString());

            return result;
        }
    }
}