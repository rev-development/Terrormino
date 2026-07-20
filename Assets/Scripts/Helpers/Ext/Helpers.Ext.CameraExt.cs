using Helpers.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace Helpers.Ext
{
	[PublicAPI]
	public static class CameraExt
	{
		/// <summary>
		///     Calculates the 3D world position of a normalized viewport point at a specific distance slice from the camera.
		/// </summary>
		/// <param name="camera">The target <see cref="Camera" /> component used to evaluate the frustum geometries.</param>
		/// <param name="distance">
		///     The positive depth distance (in world units) along the camera's forward axis where the slice is
		///     located.
		/// </param>
		/// <param name="viewportPoint">
		///     A normalized coordinate representing a position on the screen.
		///     <para>
		///         <c>(0,0)</c> is the bottom-left corner, <c>(1,1)</c> is the top-right corner, and <c>(0.5,0.5)</c> is the
		///         center.
		///     </para>
		/// </param>
		/// <returns>
		///     A <see cref="Vector3" /> position in world space corresponding to the input viewport coordinates at the given
		///     distance depth.
		/// </returns>
		/// <remarks>
		///     This function automatically clamps the <paramref name="viewportPoint" /> values between 0.0f and 1.0f
		///     to ensure the returned point remains strictly within the visual boundaries of the camera frustum.
		/// </remarks>
		[AiGenerated("Gemini", "7-20-26", "Reviewed by Rev 7-20-26")]
		public static Vector3 GetPointInFrustum(this Camera camera, float distance, Vector2 viewportPoint)
		{
			var clampedViewportPoint = new Vector2(Mathf.Clamp01(viewportPoint.x), Mathf.Clamp01(viewportPoint.y));

			// 1. Calculate full dimensions at this distance slice
			var frustum = camera.GetFrustumSizeAtDistance(distance);

			// 2. Find the center point of the slice plane
			var sliceCenter = camera.transform.position + camera.transform.forward * distance;

			// 3. Shift from the center to the bottom-left corner of the slice
			var bottomLeft = sliceCenter
							 - camera.transform.right * (frustum.x * 0.5f)
							 - camera.transform.up * (frustum.y * 0.5f);

			// 4. Extrapolate across the plane using your normalized (0 to 1) coordinates
			var targetPoint = bottomLeft
							  + camera.transform.right * (frustum.x * clampedViewportPoint.x)
							  + camera.transform.up * (frustum.y * clampedViewportPoint.y);

			return targetPoint;
		}

		public static Vector3 GetPointInFrustum(this Camera camera, float distance) =>
			camera.GetPointInFrustum(distance, new Vector2(0.5f, 0.5f));

		public static Vector2 GetFrustumSizeAtDistance(this Camera camera, float distance)
		{
			// Math-wise, we're taking the cone and splitting it down the middle so we have a right triangle 
			// Height = 2 x distance x tan(FOV x 0.5 x Deg2Rad)
			// Camera.fieldOfView is the total vertical angle of the camera
			// We cut it in half to get a right triangle between the camera, the center of the frustum, and the top edge of the frustum
			// Mathf.Deg2Rad is just pi/180 which converts angles ot radians for the trig func
			// This gets us a ratio that we can then scale by multiplying it with the distance
			// We multiply 2.0f because all the math was done with a right triangle that spanned half the height
			var height = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
			var width = height * camera.aspect;

			return new Vector2(width, height);
		}

		/// <summary>
		///     Calculates the distance at which <paramref name="bounds" /> fits within the camera's frame.
		/// </summary>
		/// <param name="camera">The target <see cref="Camera" /> component used to evaluate the frustum geometries.</param>
		/// <param name="bounds">The world-space bounds to fit within the frame.</param>
		/// <param name="fillPercent">
		///     The fraction of the binding axis (whichever of height/width requires the larger distance to fit) that
		///     should be visible in frame. <c>1.0</c> (default) fits the bounds exactly; values below <c>1.0</c> pull the
		///     camera closer, cropping the binding axis; values above <c>1.0</c> pull it back, adding margin.
		/// </param>
		/// <returns>The distance, in world units, at which the camera should be placed from <paramref name="bounds" />.</returns>
		/// <remarks>
		///     <paramref name="fillPercent" /> is only exact on the binding axis. The other axis may show a different
		///     visible fraction, since frustum width and height are locked together by the camera's aspect ratio and
		///     can't both be scaled independently by a single percentage unless <paramref name="bounds" /> shares that
		///     aspect ratio.
		/// </remarks>
		[AiGenerated("Gemini", "7-20-26")]
		[AiGenerated("Claude", "Sonnet 5")]
		public static float GetDistanceToFitInFrame(this Camera camera, Bounds bounds, float fillPercent = 1.0f)
		{
			// 1. Extract the vertical half-angle of the camera in radians
			var halfFovRad = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;

			// 2. Calculate the distance needed to fit the object's vertical height
			var distanceForHeight = bounds.size.y * 0.5f / Mathf.Tan(halfFovRad);

			// 3. Calculate the horizontal half-angle using the camera's aspect ratio
			// Horizontal FOV = 2 * arctan(tan(Vertical FOV / 2) * aspect)
			var halfHorizontalFovRad = Mathf.Atan(Mathf.Tan(halfFovRad) * camera.aspect);

			// 4. Calculate the distance needed to fit the object's horizontal width
			var distanceForWidth = bounds.size.x * 0.5f / Mathf.Tan(halfHorizontalFovRad);

			// 5. Return the larger distance of the two (binding axis), scaled by the requested fill percentage
			return Mathf.Max(distanceForHeight, distanceForWidth) * fillPercent;
		}
	}
}