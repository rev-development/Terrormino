using UnityEngine;

namespace Helpers
{
    public static class Shaders
    {

        public static void ChangeSimpleColor(Color color, GameObject gameObject) {
            if (!gameObject.TryGetComponent(out MeshRenderer meshRenderer)) return;

            if (meshRenderer.material.shader.name == "SimpleColor")
            {
                meshRenderer.material.SetColor("_Color", color);
            }
        }

    }
}