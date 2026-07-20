using System.Linq;
using Helpers.Ext;
using UnityEngine;

namespace Helpers
{
	public static class Positioning
	{
		/// <summary>
		///     Aligns the top of one object's bounds to the top of another object's bounds.
		/// </summary>
		/// <param name="toBeAligned"></param>
		/// <param name="toBeAlignedTo"></param>
		public static void AlignTops(GameObject toBeAligned, GameObject toBeAlignedTo)
		{
			var collidersA = toBeAligned.GetComponentsInChildren<Collider>();
			var collidersB = toBeAlignedTo.GetComponentsInChildren<Collider>();

			if (collidersA.Length == 0
				|| collidersB.Length == 0)
			{
				Debug.LogWarning("AlignTops: one or both objects have no colliders.");

				return;
			}

			var topOfToBeAligned = collidersA.Select(collider => collider.bounds).EncapsulateMany().max.y;

			var topOfToBeAlignedTo = collidersB.Select(collider => collider.bounds).EncapsulateMany().max.y;

			var heightDiff = topOfToBeAlignedTo - topOfToBeAligned;

			var newPosition = toBeAligned.transform.position;
			newPosition.y += heightDiff;
			toBeAligned.transform.position = newPosition;
		}

		/// <summary>
		///     Aligns the top of one object's bounds to a y value.
		/// </summary>
		/// <param name="toBeAligned"></param>
		/// <param name="toBeAlignedTo"></param>
		public static void AlignTops(GameObject toBeAligned, float toBeAlignedTo)
		{
			var colliders = toBeAligned.GetComponentsInChildren<Collider>();

			if (colliders.Length == 0)
			{
				Debug.LogWarning("AlignTops: object has no colliders.");

				return;
			}

			var topOfToBeAligned = colliders.Select(collider => collider.bounds).EncapsulateMany().max.y;
			var heightDiff = toBeAlignedTo - topOfToBeAligned;

			var newPosition = toBeAligned.transform.position;
			newPosition.y += heightDiff;
			toBeAligned.transform.position = newPosition;
		}
	}
}