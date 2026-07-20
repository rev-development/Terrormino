using System.Collections.Generic;
using UnityEngine;

namespace Helpers.Ext
{
	public static class DictionaryExt
	{
		public static void AddOrUpdate<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
		{
			if (!dict.TryAdd(key, value)) dict[key] += value;
		}

		public static void AddOrUpdate<TKey>(this Dictionary<TKey, float> dict, TKey key, float value)
		{
			if (!dict.TryAdd(key, value)) dict[key] += value;
		}

		public static void AddOrUpdate<TKey>(this Dictionary<TKey, double> dict, TKey key, double value)
		{
			if (!dict.TryAdd(key, value)) dict[key] += value;
		}

		public static void AddOrUpdate<TKey>(this Dictionary<TKey, Vector2> dict, TKey key, Vector2 value)
		{
			if (!dict.TryAdd(key, value)) dict[key] += value;
		}

		public static void AddOrUpdate<TKey>(this Dictionary<TKey, Vector2Int> dict, TKey key, Vector2Int value)
		{
			if (!dict.TryAdd(key, value)) dict[key] += value;
		}

		public static void AddOrUpdate<TKey>(this Dictionary<TKey, Vector3> dict, TKey key, Vector3 value)
		{
			if (!dict.TryAdd(key, value)) dict[key] += value;
		}

		public static void AddOrUpdate<TKey>(this Dictionary<TKey, Vector3Int> dict, TKey key, Vector3Int value)
		{
			if (!dict.TryAdd(key, value)) dict[key] += value;
		}

		public static void AddOrUpdateMany<TKey>(this Dictionary<TKey, int> dict, Dictionary<TKey, int> dictToAdd)
		{
			foreach (var kvp in dictToAdd) dict.AddOrUpdate(kvp.Key, kvp.Value);
		}

		public static void AddOrUpdateMany<TKey>(this Dictionary<TKey, float> dict, Dictionary<TKey, float> dictToAdd)
		{
			foreach (var kvp in dictToAdd) dict.AddOrUpdate(kvp.Key, kvp.Value);
		}

		public static void AddOrUpdateMany<TKey>(this Dictionary<TKey, double> dict, Dictionary<TKey, double> dictToAdd)
		{
			foreach (var kvp in dictToAdd) dict.AddOrUpdate(kvp.Key, kvp.Value);
		}

		public static void AddOrUpdateMany<TKey>(this Dictionary<TKey, Vector2> dict, Dictionary<TKey, Vector2> dictToAdd)
		{
			foreach (var kvp in dictToAdd) dict.AddOrUpdate(kvp.Key, kvp.Value);
		}

		public static void AddOrUpdateMany<TKey>(this Dictionary<TKey, Vector2Int> dict, Dictionary<TKey, Vector2Int> dictToAdd)
		{
			foreach (var kvp in dictToAdd) dict.AddOrUpdate(kvp.Key, kvp.Value);
		}

		public static void AddOrUpdateMany<TKey>(this Dictionary<TKey, Vector3> dict, Dictionary<TKey, Vector3> dictToAdd)
		{
			foreach (var kvp in dictToAdd) dict.AddOrUpdate(kvp.Key, kvp.Value);
		}

		public static void AddOrUpdateMany<TKey>(this Dictionary<TKey, Vector3Int> dict, Dictionary<TKey, Vector3Int> dictToAdd)
		{
			foreach (var kvp in dictToAdd) dict.AddOrUpdate(kvp.Key, kvp.Value);
		}
	}
}