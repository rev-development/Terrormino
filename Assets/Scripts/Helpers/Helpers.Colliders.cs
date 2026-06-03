using UnityEngine;

namespace Helpers
{
    public static class Colliders
    {
        // TODO: Figure out if redundant
        public static Collider FindAnyCollider(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out BoxCollider boxCollider)) return boxCollider;

            if (gameObject.TryGetComponent(out CapsuleCollider capsuleCollider)) return capsuleCollider;

            if (gameObject.TryGetComponent(out MeshCollider meshCollider)) return meshCollider;

            if (gameObject.TryGetComponent(out SphereCollider sphereCollider)) return sphereCollider;

            if (gameObject.TryGetComponent(out TerrainCollider terrainCollider)) return terrainCollider;

            if (gameObject.TryGetComponent(out WheelCollider wheelCollider)) return wheelCollider;

            return null;
        }
    }
}