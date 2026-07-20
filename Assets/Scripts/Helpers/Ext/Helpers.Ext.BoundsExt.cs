using System.Collections.Generic;
using UnityEngine;

namespace Helpers.Ext
{
	public static class BoundsExt
	{
		public static Bounds EncapsulateMany(this Bounds bounds, params Bounds[] boundsToEncapsulate)
		{
			var newBounds = bounds;

			foreach (var boundToEncapsulate in boundsToEncapsulate) newBounds.Encapsulate(boundToEncapsulate);

			return newBounds;
		}

		public static Bounds EncapsulateMany(this IEnumerable<Bounds> boundsCollection)
		{
			using var enumerator = boundsCollection.GetEnumerator();

			if (!enumerator.MoveNext())
			{
				Debug.LogWarning("EncapsulateMany called on an empty collection.");

				return default;
			}

			var result = enumerator.Current;

			while (enumerator.MoveNext())
			{
				result.Encapsulate(enumerator.Current);
			}

			return result;
		}

		public static Vector3 SampleRandom3DPosition(this Bounds bounds) =>
			new(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y),
				Random.Range(bounds.min.z, bounds.max.z)
			);

		public static Vector3 SampleRandom2DPosition(this Bounds bounds) =>
			new(Random.Range(bounds.min.x, bounds.max.x), bounds.center.y, Random.Range(bounds.min.z, bounds.max.z));
	}
}