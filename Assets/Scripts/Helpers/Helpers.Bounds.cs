using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Helpers
{
    public static class Bounds
    {
        public static Vector3 GetRandomPositionInCollider(Collider collider)
        {
            return new Vector3(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                Random.Range(collider.bounds.min.z, collider.bounds.min.z)
            );
        }

        public static Vector3 GetRandomPositionOnNavMesh(NavMeshSurface navMesh)
        {
            var randomBounds = new UnityEngine.Bounds();

            randomBounds.SetMinMax(navMesh.navMeshData.sourceBounds.min,
                navMesh.navMeshData.sourceBounds.max);

            var randomPosition = new Vector3(
                Random.Range(randomBounds.min.x, randomBounds.max.x),
                Random.Range(randomBounds.min.y, randomBounds.max.y),
                Random.Range(randomBounds.min.z, randomBounds.max.z)
            );

            NavMesh.SamplePosition(randomPosition, out var navMeshHit, Mathf.Infinity, navMesh.layerMask);

            return navMeshHit.position;
        }

        public static Vector3 GetRandomPositionOnNavMesh(NavMeshSurface navMesh, Collider excludeCollider,
            int excludeColliderMultiplier)
        {
            var randomBounds = new UnityEngine.Bounds();

            randomBounds.SetMinMax(excludeCollider.bounds.max * excludeColliderMultiplier,
                navMesh.navMeshData.sourceBounds.max);

            var randomPosition = new Vector3(
                Random.Range(randomBounds.min.x, randomBounds.max.x),
                Random.Range(randomBounds.min.y, randomBounds.max.y),
                Random.Range(randomBounds.min.z, randomBounds.max.z)
            );


            NavMesh.SamplePosition(randomPosition, out var navMeshHit, Mathf.Infinity, navMesh.layerMask);
            return navMeshHit.position;
        }
    }
}