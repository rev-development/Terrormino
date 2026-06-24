using UnityEngine;

namespace Helpers
{
	public static class Positioning
	{
		/// <summary>
		///     Positions an object in front of another object, based on that object's forward vector.
		/// </summary>
		/// <param name="toBePositioned"></param>
		/// <param name="toBePositionedInFrontOf"></param>
		/// <param name="howFarInFront"></param>
		public static void PositionInFrontOf(
			Transform toBePositioned,
			Transform toBePositionedInFrontOf,
			float howFarInFront
		)
		{
			var newPosition = toBePositionedInFrontOf.transform.position
							  + toBePositionedInFrontOf.transform.position * howFarInFront;

			toBePositioned.transform.position = newPosition;
		}

		/// <summary>
		///     Aligns the top of one object's bounds to the top of another object's bounds.
		/// </summary>
		/// <param name="toBeAligned"></param>
		/// <param name="toBeAlignedTo"></param>
		public static void AlignTops(GameObject toBeAligned, GameObject toBeAlignedTo)
		{
			var topOfToBeAligned = Bounds.GetComplexBounds(toBeAligned).max.y;
			var topOfToBeAlignedTo = Bounds.GetComplexBounds(toBeAlignedTo).max.y;
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
			var topOfToBeAligned = Bounds.GetComplexBounds(toBeAligned).max.y;
			var heightDiff = toBeAlignedTo - topOfToBeAligned;

			var newPosition = toBeAligned.transform.position;
			newPosition.y += heightDiff;
			toBeAligned.transform.position = newPosition;
		}
	}
}