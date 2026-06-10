using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace AYellowpaper.SerializedCollections.Editor
{
    internal static class SCEnumUtility
    {

        private static readonly Dictionary<Type, EnumCache> _cache = new();

        internal static EnumCache GetEnumCache(Type enumType)
        {
            if (_cache.TryGetValue(enumType, out var val))
            {
                return val;
            }

            var classType = typeof(EditorGUI).Assembly.GetType("UnityEditor.EnumDataUtility");
            var methodInfo = classType.GetMethod("GetCachedEnumData", BindingFlags.Static | BindingFlags.NonPublic);

            object[] parameters =
            {
                enumType,
                true
            };

            object result = methodInfo.Invoke(null, parameters);
            int[] flagValues = (int[])result.GetType().GetField("flagValues").GetValue(result);
            string[] names = (string[])result.GetType().GetField("names").GetValue(result);
            var cache = new EnumCache(enumType, flagValues, names);
            _cache.Add(enumType, cache);

            return cache;
        }

    }

    internal record EnumCache
    {

        private readonly Dictionary<int, string[]> _namesByValue = new();

        public readonly int[] FlagValues;

        public readonly bool IsFlag;

        public readonly int Length;

        public readonly string[] Names;

        public readonly Type Type;

        public EnumCache(Type type, int[] flagValues, string[] displayNames)
        {
            Type = type;
            FlagValues = flagValues;
            Names = displayNames;
            Length = flagValues.Length;
            IsFlag = Type.IsDefined(typeof(FlagsAttribute));
        }

        internal string[] GetNamesForValue(int value)
        {
            if (_namesByValue.TryGetValue(value, out string[] list))
            {
                return list;
            }

            string[] array = IsFlag
                ? GetFlagValues(value).ToArray()
                : new[]
                  {
                      GetEnumValue(value)
                  };

            _namesByValue.Add(value, array);

            return array;
        }

        private string GetEnumValue(int value)
        {
            for (int i = 0; i < Length; i++)
            {
                if (FlagValues[i] == value)
                {
                    return Names[i];
                }
            }

            return null;
        }

        private IEnumerable<string> GetFlagValues(int flagValue)
        {
            if (flagValue == 0)
            {
                yield return FlagValues[0] == 0 ? Names[0] : "Nothing";

                yield break;
            }

            for (int i = 0; i < Length; i++)
            {
                int fv = FlagValues[i];

                if ((fv & flagValue) == fv
                    && fv != 0)
                {
                    yield return Names[i];
                }
            }

            if (FlagValues[Length - 1] != -1
                && flagValue == -1)
            {
                yield return "Everything";
            }
        }

    }
}