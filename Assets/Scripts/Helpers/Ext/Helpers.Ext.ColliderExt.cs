using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Helpers.Ext
{
	public static class ColliderExt
	{
		public static Vector3 SampleRandomPosition(this Collider collider) =>
			new(
				Random.Range(collider.bounds.min.x, collider.bounds.max.x),
				Random.Range(collider.bounds.min.y, collider.bounds.max.y),
				Random.Range(collider.bounds.min.z, collider.bounds.min.z)
			);

		public static Bounds GetAllBounds(this IEnumerable<Collider> colliders) =>
			colliders.Select(collider => collider.bounds).EncapsulateMany();
	}
}