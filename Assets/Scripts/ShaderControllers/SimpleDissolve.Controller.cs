using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleDissolve
{
    public float DissolveValue = 0f;
    private List<Material> _materials = new();

    public void Init(GameObject gameObject)
    {
        _materials.AddRange(
            Helpers
                .Debug.TryFindComponentsInChildren<SkinnedMeshRenderer>(gameObject)
                .Select(renderer => renderer.material)
        );

        _materials.AddRange(
            Helpers
                .Debug.TryFindComponentsInChildren<MeshRenderer>(gameObject)
                .Select(renderer => renderer.material)
        );
    }

    public void Dissolve(float deltaTime)
    {
        DissolveValue = Mathf.Clamp01(DissolveValue + (deltaTime * 1f));
        _materials.ForEach(material => material.SetFloat("_DissolveValue", DissolveValue));
    }
}
