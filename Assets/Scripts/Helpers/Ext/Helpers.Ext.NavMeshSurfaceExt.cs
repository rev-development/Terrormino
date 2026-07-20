using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Helpers.Ext
{
	public static class NavMeshSurfaceExt
	{
		public static Vector3 SampleRandomPosition(this NavMeshSurface navMesh)
		{
			var randomBounds = new Bounds();

			randomBounds.SetMinMax(navMesh.navMeshData.sourceBounds.min, navMesh.navMeshData.sourceBounds.max);

			var randomPosition = new Vector3(
				Random.Range(randomBounds.min.x, randomBounds.max.x),
				Random.Range(randomBounds.min.y, randomBounds.max.y),
				Random.Range(randomBounds.min.z, randomBounds.max.z)
			);

			NavMesh.SamplePosition(
				randomPosition,
				out var navMeshHit,
				Mathf.Infinity,
				navMesh.layerMask
			);

			return navMeshHit.position;
		}

		public static Vector3 SampleRandomPosition(
			this NavMeshSurface navMesh,
			Collider excludeCollider,
			int excludeColliderMultiplier
		)
		{
			var randomBounds = new Bounds();

			randomBounds.SetMinMax(
				excludeCollider.bounds.max * excludeColliderMultiplier,
				navMesh.navMeshData.sourceBounds.max
			);

			var randomPosition = new Vector3(
				Random.Range(randomBounds.min.x, randomBounds.max.x),
				Random.Range(randomBounds.min.y, randomBounds.max.y),
				Random.Range(randomBounds.min.z, randomBounds.max.z)
			);

			NavMesh.SamplePosition(
				randomPosition,
				out var navMeshHit,
				Mathf.Infinity,
				navMesh.layerMask
			);

			return navMeshHit.position;
		}
	}
}