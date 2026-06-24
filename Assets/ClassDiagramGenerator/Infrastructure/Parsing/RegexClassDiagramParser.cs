using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClassDiagramGenerator.Application.Abstractions;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Infrastructure.Parsing
{
    public sealed class RegexClassDiagramParser : IClassDiagramParser
    {
        private static readonly Regex TypeHeaderRegex = new(
            @"(?<summary>(?:\s*///.*\r?\n)*)\s*(?<modifiers>(?:(?:public|private|protected|internal|abstract|sealed|static|partial)\s+)*)\b(?<kind>class|interface|struct)\s+(?<name>[A-Za-z_]\w*)(?<generics>\s*<[^>{;]+>)?\s*(?:\:\s*(?<bases>[^{\r\n]+))?\s*\{",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex NamespaceRegex = new(
            @"\bnamespace\s+(?<name>[A-Za-z_][\w\.]*)",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex FieldRegex = new(
            @"^\s*(?<visibility>public|private|protected|internal)\s+(?:(?:static|readonly|const|volatile|new)\s+)*(?<type>[A-Za-z_][\w<>\[\],\.\?]+)\s+(?<name>[A-Za-z_]\w*)\s*(?:=\s*[^;]+)?;",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex PropertyRegex = new(
            @"^\s*(?<visibility>public|private|protected|internal)\s+(?:(?:static|virtual|abstract|override|sealed|new)\s+)*(?<type>[A-Za-z_][\w<>\[\],\.\?]+)\s+(?<name>[A-Za-z_]\w*)\s*\{\s*(?:(?:public|private|protected|internal)\s+)?get\s*;\s*(?:(?:public|private|protected|internal)\s+)?set\s*;\s*\}",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex MethodRegex = new(
            @"^\s*(?<visibility>public|private|protected|internal)\s+(?:(?:static|virtual|abstract|override|sealed|async|new|partial)\s+)*(?<returnType>[A-Za-z_][\w<>\[\],\.\?]+)\s+(?<name>[A-Za-z_]\w*)\s*\((?<params>[^\)]*)\)\s*(?:where[^\{]+)?(?=\{|;)",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex MethodCallRegex = new(
            @"\b(?<name>[A-Za-z_]\w*)\s*\(",
            RegexOptions.Compiled);

        public IReadOnlyList<DiagramTypeModel> ParseFile(string sourceText)
        {
            var types = new List<DiagramTypeModel>();
            if (string.IsNullOrWhiteSpace(sourceText))
                return types;

            string fileNamespace = ExtractNamespace(sourceText);

            foreach (Match match in TypeHeaderRegex.Matches(sourceText))
            {
                bool isInterface = match.Groups["kind"].Value == "interface";
                string typeName = match.Groups["name"].Value;
                string summary = CleanSummary(match.Groups["summary"].Value);

                string baseType = null;
                List<string> interfaces = new();

                if (match.Groups["bases"].Success)
                {
                    var bases = SafeSplitBaseTypes(match.Groups["bases"].Value);
                    if (bases.Count > 0)
                    {
                        if (!isInterface)
                        {
                            baseType = bases[0];
                            if (bases.Count > 1)
                                interfaces.AddRange(bases.Skip(1));
                        }
                        else
                        {
                            interfaces.AddRange(bases);
                        }
                    }
                }

                int bodyOpenBraceIndex = match.Index + match.Length - 1;
                int bodyCloseBraceIndex = FindMatchingBrace(sourceText, bodyOpenBraceIndex);

                if (bodyCloseBraceIndex <= bodyOpenBraceIndex)
                    continue;

                string body = sourceText.Substring(bodyOpenBraceIndex + 1, bodyCloseBraceIndex - bodyOpenBraceIndex - 1);

                var type = new DiagramTypeModel
                {
                    Name = typeName,
                    Namespace = fileNamespace,
                    BaseType = baseType,
                    Interfaces = interfaces,
                    IsAbstract = match.Groups["modifiers"].Value.Contains("abstract"),
                    IsInterface = isInterface,
                    Summary = summary
                };

                ParseMembers(body, type);
                ExtractMethodCalls(body, type);
                types.Add(type);
            }

            return types;
        }

        private static string ExtractNamespace(string content)
        {
            Match match = NamespaceRegex.Match(content);
            return match.Success ? match.Groups["name"].Value : string.Empty;
        }

        private static void ParseMembers(string body, DiagramTypeModel type)
        {
            string topLevel = StripNestedTypeBodies(body);

            foreach (Match match in FieldRegex.Matches(topLevel))
            {
                type.Fields.Add(new DiagramFieldModel
                {
                    Visibility = GetVisibilitySymbol(match.Groups["visibility"].Value),
                    Type = match.Groups["type"].Value,
                    Name = match.Groups["name"].Value
                });
            }

            foreach (Match match in PropertyRegex.Matches(topLevel))
            {
                type.Properties.Add(new DiagramPropertyModel
                {
                    Visibility = GetVisibilitySymbol(match.Groups["visibility"].Value),
                    Type = match.Groups["type"].Value,
                    Name = match.Groups["name"].Value
                });
            }

            foreach (Match match in MethodRegex.Matches(topLevel))
            {
                if (type.Name == match.Groups["name"].Value)
                    continue;

                type.Methods.Add(new DiagramMethodModel
                {
                    Visibility = GetVisibilitySymbol(match.Groups["visibility"].Value),
                    ReturnType = string.IsNullOrWhiteSpace(match.Groups["returnType"].Value)
                        ? "void"
                        : match.Groups["returnType"].Value,
                    Name = match.Groups["name"].Value,
                    Parameters = ParseParameters(match.Groups["params"].Value)
                });
            }
        }

        private static void ExtractMethodCalls(string body, DiagramTypeModel type)
        {
            var methodNames = new HashSet<string>(type.Methods.Select(m => m.Name));
            foreach (Match match in MethodCallRegex.Matches(body))
            {
                string calledName = match.Groups["name"].Value;
                // Si ce n'est pas une méthode définie dans cette classe, on l'ajoute comme appel potentiel à une autre classe
                if (!methodNames.Contains(calledName))
                {
                    if (!type.LocalCalls.Contains(calledName))
                        type.LocalCalls.Add(calledName);
                }
            }
        }

        private static List<string> SafeSplitBaseTypes(string input)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(input))
                return result;

            var sb = new StringBuilder();
            int depth = 0;

            foreach (char character in input)
            {
                if (character == '<')
                {
                    depth++;
                    sb.Append(character);
                }
                else if (character == '>')
                {
                    depth = Math.Max(0, depth - 1);
                    sb.Append(character);
                }
                else if (character == ',' && depth == 0)
                {
                    result.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else
                {
                    sb.Append(character);
                }
            }

            if (sb.Length > 0)
                result.Add(sb.ToString().Trim());

            return result;
        }

        private static string StripNestedTypeBodies(string body)
        {
            var sb = new StringBuilder(body.Length);
            int depth = 0;

            foreach (char character in body)
            {
                if (character == '{')
                {
                    depth++;
                    sb.Append('\n');
                }
                else if (character == '}')
                {
                    depth = Math.Max(0, depth - 1);
                    sb.Append('\n');
                }
                else
                {
                    if (depth == 0)
                        sb.Append(character);
                    else if (character == '\n' || character == '\r')
                        sb.Append(character);
                    else
                        sb.Append(' ');
                }
            }

            return sb.ToString();
        }

        private static int FindMatchingBrace(string text, int openBraceIndex)
        {
            if (openBraceIndex < 0 || openBraceIndex >= text.Length || text[openBraceIndex] != '{')
                return -1;

            int depth = 0;
            bool inString = false;
            bool inChar = false;
            bool inLineComment = false;
            bool inBlockComment = false;

            for (int index = openBraceIndex; index < text.Length; index++)
            {
                char current = text[index];
                char next = index + 1 < text.Length ? text[index + 1] : '\0';
                char previous = index > 0 ? text[index - 1] : '\0';

                if (inLineComment)
                {
                    if (current == '\n')
                        inLineComment = false;
                    continue;
                }

                if (inBlockComment)
                {
                    if (previous == '*' && current == '/')
                        inBlockComment = false;
                    continue;
                }

                if (!inString && !inChar)
                {
                    if (current == '/' && next == '/')
                    {
                        inLineComment = true;
                        index++;
                        continue;
                    }

                    if (current == '/' && next == '*')
                    {
                        inBlockComment = true;
                        index++;
                        continue;
                    }
                }

                if (!inChar && current == '"' && previous != '\\')
                {
                    inString = !inString;
                    continue;
                }

                if (!inString && current == '\'' && previous != '\\')
                {
                    inChar = !inChar;
                    continue;
                }

                if (inString || inChar)
                    continue;

                if (current == '{')
                {
                    depth++;
                }
                else if (current == '}')
                {
                    depth--;
                    if (depth == 0)
                        return index;
                }
            }

            return -1;
        }

        private static string CleanSummary(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            string[] lines = raw
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Select(line => line.StartsWith("///") ? line.Substring(3).Trim() : line)
                .ToArray();

            return string.Join(" ", lines).Trim();
        }

        private static string GetVisibilitySymbol(string keyword)
        {
            if (keyword.Contains("public")) return "+";
            if (keyword.Contains("private")) return "-";
            if (keyword.Contains("protected")) return "#";
            if (keyword.Contains("internal")) return "~";
            return string.Empty;
        }

        private static List<DiagramParameterModel> ParseParameters(string raw)
        {
            var list = new List<DiagramParameterModel>();
            if (string.IsNullOrWhiteSpace(raw))
                return list;

            foreach (string parameter in SplitParameters(raw))
            {
                string cleaned = parameter.Trim();
                if (string.IsNullOrWhiteSpace(cleaned))
                    continue;

                string[] tokens = cleaned
                    .Replace("ref ", string.Empty)
                    .Replace("out ", string.Empty)
                    .Replace("in ", string.Empty)
                    .Replace("params ", string.Empty)
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length < 2)
                    continue;

                string name = tokens[^1];
                string type = string.Join(" ", tokens.Take(tokens.Length - 1));

                if (name.Contains("="))
                    name = name.Split('=')[0].Trim();

                list.Add(new DiagramParameterModel
                {
                    Type = type.Trim(),
                    Name = name.Trim()
                });
            }

            return list;
        }

        private static List<string> SplitParameters(string raw)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(raw))
                return result;

            var sb = new StringBuilder();
            int genericDepth = 0;
            int parenthesisDepth = 0;
            int bracketDepth = 0;

            foreach (char character in raw)
            {
                switch (character)
                {
                    case '<': genericDepth++; sb.Append(character); break;
                    case '>': genericDepth = Math.Max(0, genericDepth - 1); sb.Append(character); break;
                    case '(': parenthesisDepth++; sb.Append(character); break;
                    case ')': parenthesisDepth = Math.Max(0, parenthesisDepth - 1); sb.Append(character); break;
                    case '[': bracketDepth++; sb.Append(character); break;
                    case ']': bracketDepth = Math.Max(0, bracketDepth - 1); sb.Append(character); break;
                    case ',':
                        if (genericDepth == 0 && parenthesisDepth == 0 && bracketDepth == 0)
                        {
                            result.Add(sb.ToString());
                            sb.Clear();
                        }
                        else
                        {
                            sb.Append(character);
                        }
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }

            if (sb.Length > 0)
                result.Add(sb.ToString());

            return result;
        }
    }
}