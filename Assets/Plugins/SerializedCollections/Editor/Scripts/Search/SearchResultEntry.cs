using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace AYellowpaper.SerializedCollections.Editor.Search
{
    public class SearchResultEntry
    {

        public readonly int Index;

        public readonly IEnumerable<PropertySearchResult> MatchingResults;

        public readonly SerializedProperty Property;

        public SearchResultEntry(
            int index,
            SerializedProperty property,
            IEnumerable<PropertySearchResult> matchingResults
        )
        {
            Index = index;
            Property = property;
            MatchingResults = matchingResults;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Index}: {Property.propertyPath}");

            foreach (var matchingResult in MatchingResults)
            {
                sb.AppendLine(matchingResult.ToString());
            }

            return sb.ToString();
        }

    }
}