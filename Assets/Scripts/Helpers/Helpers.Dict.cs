using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global

namespace Helpers
{
    public static class Dict
    {

        public static void AddOrUpdate(Dictionary<string, float> dict, string key, float value) {
            if (!dict.TryAdd(key, value))
            {
                dict[key] += value;
            }
        }

        public static Dictionary<string, float> Collate(
            Dictionary<string, float> baseDict,
            Dictionary<string, float> dictToAppend
        ) {
            foreach (var keyValuePair in dictToAppend)
            {
                AddOrUpdate(baseDict, keyValuePair.Key, keyValuePair.Value);
            }

            return baseDict;
        }

        public static Dictionary<string, T> Collate<T>(
            Dictionary<string, T> baseDict,
            Dictionary<string, T> dictToAppend
        ) {
            foreach (var keyValuePair in dictToAppend)
            {
                baseDict.TryAdd(keyValuePair.Key, keyValuePair.Value);
            }

            return baseDict;
        }

    }
}