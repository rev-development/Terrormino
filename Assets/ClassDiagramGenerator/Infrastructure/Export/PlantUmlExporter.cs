using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassDiagramGenerator.Application.Abstractions;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Infrastructure.Export
{
    public sealed class PlantUmlExporter : IPlantUmlExporter
    {
        public string Export(IReadOnlyList<DiagramTypeModel> types, bool includeAssociations)
        {
            var sb = new StringBuilder();
            sb.AppendLine("@startuml");

            foreach (var type in types)
            {
                string typeKeyword = type.IsInterface ? "interface" : "class";
                string stereotype = !type.IsInterface && type.IsAbstract ? " <<abstract>>" : string.Empty;

                sb.AppendLine($"{typeKeyword} {type.Name}{stereotype} {{");

                foreach (var field in type.Fields)
                    sb.AppendLine($"    {field.Visibility} {field.Name} : {field.Type}");

                foreach (var property in type.Properties)
                    sb.AppendLine($"    {property.Visibility} {property.Name} : {property.Type} {{ get; set; }}");

                foreach (var method in type.Methods)
                {
                    string parameterList = string.Join(", ", method.Parameters.Select(p => $"{p.Name} : {p.Type}"));
                    sb.AppendLine($"    {method.Visibility} {method.Name}({parameterList}) : {method.ReturnType}");
                }

                sb.AppendLine("}");

                if (!string.IsNullOrWhiteSpace(type.Summary))
                    sb.AppendLine($"' {type.Name}: {type.Summary}");
            }

            foreach (var type in types)
            {
                if (!string.IsNullOrWhiteSpace(type.BaseType))
                    sb.AppendLine($"{StripGenerics(type.BaseType)} <|-- {type.Name}");

                foreach (string iface in type.Interfaces)
                    sb.AppendLine($"{StripGenerics(iface)} <|.. {type.Name}");
            }

            if (includeAssociations)
                AppendAssociations(sb, types);

            sb.AppendLine("@enduml");
            return sb.ToString();
        }

        private static void AppendAssociations(StringBuilder sb, IReadOnlyList<DiagramTypeModel> types)
        {
            var names = new HashSet<string>(types.Select(type => StripGenerics(type.Name)));
            
            // On regroupe les associations par (Source, Cible) pour éviter les lignes multiples
            // Key: (SourceClassName, TargetClassName), Value: List of member names
            var associations = new Dictionary<(string, string), List<string>>();

            foreach (var type in types)
            {
                string className = StripGenerics(type.Name);

                foreach (var field in type.Fields)
                {
                    string typeName = StripGenerics(field.Type);
                    if (names.Contains(typeName) && typeName != className)
                    {
                        var key = (className, typeName);
                        if (!associations.ContainsKey(key)) associations[key] = new List<string>();
                        if (!associations[key].Contains(field.Name))
                            associations[key].Add(field.Name);
                    }
                }

                foreach (var property in type.Properties)
                {
                    string typeName = StripGenerics(property.Type);
                    if (names.Contains(typeName) && typeName != className)
                    {
                        var key = (className, typeName);
                        if (!associations.ContainsKey(key)) associations[key] = new List<string>();
                        if (!associations[key].Contains(property.Name))
                            associations[key].Add(property.Name);
                    }
                }

                foreach (var method in type.Methods)
                {
                    foreach (var parameter in method.Parameters)
                    {
                        string typeName = StripGenerics(parameter.Type);
                        if (names.Contains(typeName) && typeName != className)
                        {
                            var key = (className, typeName);
                            if (!associations.ContainsKey(key)) associations[key] = new List<string>();
                            // On ajoute un suffixe () pour les paramètres de méthode si on veut être précis, 
                            // ou juste le nom du paramètre. Ici on met le nom de la méthode pour plus de clarté sur "l'usage"
                            string label = $"{method.Name}({parameter.Name})";
                            if (!associations[key].Contains(label))
                                associations[key].Add(label);
                        }
                    }
                }

                // NOUVEAU: On ajoute les appels de méthodes détectés dans le corps
                foreach (var call in type.LocalCalls)
                {
                    // On cherche quelle classe possède une méthode avec ce nom
                    foreach (var targetType in types)
                    {
                        if (targetType == type) continue;
                        if (targetType.Methods.Any(m => m.Name == call))
                        {
                            string targetName = StripGenerics(targetType.Name);
                            var key = (className, targetName);
                            if (!associations.ContainsKey(key)) associations[key] = new List<string>();
                            string label = $"{call}()";
                            if (!associations[key].Contains(label))
                                associations[key].Add(label);
                        }
                    }
                }
            }

            foreach (var kvp in associations)
            {
                string source = kvp.Key.Item1;
                string target = kvp.Key.Item2;
                string members = string.Join(", ", kvp.Value.Take(5)); // On limite à 5 pour ne pas surcharger
                if (kvp.Value.Count > 5) members += "...";

                sb.AppendLine($"{source} --> {target} : {members}");
            }
        }

        private static string StripGenerics(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return typeName;

            int index = typeName.IndexOf('<');
            return index >= 0 ? typeName.Substring(0, index) : typeName;
        }
    }
}