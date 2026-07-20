using UnityEngine;

namespace Helpers.Ext
{
	public static class TransformExt
	{
		/// <summary>
		///     Positions an object in front of another object, based on that object's forward vector.
		/// </summary>
		/// <param name="toBePositioned"></param>
		/// <param name="toBePositionedInFrontOf"></param>
		/// <param name="howFarInFront"></param>
		public static void PositionInFrontOf(
			this Transform toBePositioned,
			Transform toBePositionedInFrontOf,
			float howFarInFront
		)
		{
			var newPosition = toBePositionedInFrontOf.transform.position
							  + toBePositionedInFrontOf.transform.position * howFarInFront;

			toBePositioned.transform.position = newPosition;
		}
	}
}