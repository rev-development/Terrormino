using System.Collections.Generic;
using System.Linq;
using Helpers.Ext;
using UnityEngine;

public class SimpleDissolve
{
	private readonly List<Material> _materials = new();

	public float DissolveValue = 0f;

	public void Init(GameObject gameObject)
	{
		_materials.AddRange(
			gameObject.TryFindComponentsInChildren<SkinnedMeshRenderer>().Select(renderer => renderer.material)
		);

		_materials.AddRange(
			gameObject.TryFindComponentsInChildren<MeshRenderer>().Select(renderer => renderer.material)
		);
	}

	public void Dissolve(float deltaTime)
	{
		DissolveValue = Mathf.Clamp01(DissolveValue + deltaTime * 1f);
		_materials.ForEach(material => material.SetFloat("_DissolveValue", DissolveValue));
	}
}