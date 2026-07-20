using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Helpers
{
	/// <summary>
	///     Holds a list of elements and hands out random elements one at a time without
	///     repeating until every element has been chosen ("shuffle bag" pattern). Once
	///     exhausted, the bag automatically reshuffles and starts over — guaranteeing no
	///     immediate repeat at the reshuffle boundary unless the source list has 1 element.
	/// </summary>
	[Serializable]
	[Helpers.Attributes.AiGeneratedAttribute("Claude", "Sonnet 4.6")]
	public class RandomBag<T>
	{
		public bool AutoRefillOnExhaustion = true;

		[field: SerializeField] public List<T> Bag { get; protected set; }

		[field: SerializeField] public List<T> Source { get; protected set; }

		public RandomBag()
		{
		}

		public RandomBag(IEnumerable<T> source) => Init(source);

		public int RemainingCount => Bag.Count;

		public bool HasItems => Bag.Count > 0;

		public bool IsInitialized { get; private set; }

		/// <summary>
		///     Sets (or replaces) the source pool and reshuffles. Required before drawing
		///     when using the parameterless constructor. Safe to call again later to swap
		///     the pool at runtime (e.g. unlocking new variants).
		/// </summary>
		public void Init(IEnumerable<T> source)
		{
			Source = new List<T>(source);
			Bag = new List<T>();
			IsInitialized = true;
			Refill();
		}

		public T Next()
		{
			TryNext(out var result);

			return result;
		}

		/// <summary>
		///     Attempts to draw the next random unchosen element. Returns false (and logs an
		///     error) if the bag has no source elements, instead of silently handing back
		///     default(T) — which is indistinguishable from a real 0/false/struct(0) for
		///     value types. Prefer this over Next() whenever T might be a value type.
		/// </summary>
		public bool TryNext(out T result)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning("RandomBag.Next called before Init().");
				result = default;

				return false;
			}

			if (Source.Count == 0)
			{
				Debug.LogWarning("RandomBag has no source elements to choose from.");
				result = default;

				return false;
			}

			var attempts = 0;
			var maxAttempts = Source.Count;

			while (attempts < maxAttempts)
			{
				attempts++;

				if (Bag.Count == 0)
				{
					if (!AutoRefillOnExhaustion)
					{
						Debug.LogWarning("RandomBag is exhausted and AutoRefillOnExhaustion is false.");
						result = default;

						return false;
					}

					Refill();
				}

				var lastIndex = Bag.Count - 1;
				var candidate = Bag[lastIndex];
				Bag.RemoveAt(lastIndex);

				if (IsValid(candidate))
				{
					result = candidate;

					return true;
				}

				Debug.LogWarning(
					"RandomBag skipped a destroyed/null entry. Consider calling RemoveItem to clean it up."
				);

				Source.Remove(candidate);
			}

			Debug.LogWarning("RandomBag has no valid (non-destroyed) elements left to choose from.");
			result = default;

			return false;
		}

		/// <summary>
		///     Catches Unity's "fake null" case: a destroyed GameObject/Component whose
		///     C# reference is still non-null but should be treated as gone. Plain
		///     reference types use standard null checks; value types are always valid.
		/// </summary>
		private static bool IsValid(T item)
		{
			if (item is Object unityObject) return unityObject != null;

			if (typeof(T).IsValueType) return true;

#pragma warning disable S2955 // Generic parameters not constrained to reference types should not be compared to null
			return item != null;
		}

		/// <summary>
		///     Adds an item to the source pool. The item also becomes immediately
		///     drawable in the current cycle (added to the live bag), not just on
		///     the next reshuffle.
		/// </summary>
		public void AddItem(T item)
		{
			if (!IsInitialized)
			{
				Debug.LogError("RandomBag.AddItem called before Init(). Call Init(source) first.");

				return;
			}

			Source.Add(item);
			Bag.Add(item);
		}

		/// <summary>
		///     Removes the first matching item from the source pool and, if present,
		///     from the current live bag. Returns false (and logs a warning) if no
		///     matching item was found — this is treated as a caller mistake, not
		///     a hard error.
		/// </summary>
		public bool RemoveItem(T item)
		{
			if (!IsInitialized)
			{
				Debug.LogError("RandomBag.RemoveItem called before Init(). Call Init(source) first.");

				return false;
			}

			var removedFromSource = Source.Remove(item);

			if (!removedFromSource)
			{
				Debug.LogWarning("RandomBag.RemoveItem could not find a matching item in the source pool.");

				return false;
			}

			Bag.Remove(item);

			return true;
		}

		public void Refill()
		{
			Bag = new List<T>(Source);
			Shuffle(Bag);
		}

		private static void Shuffle(List<T> list)
		{
			for (var i = list.Count - 1; i > 0; i--)
			{
				var j = Random.Range(0, i + 1);
				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}
}