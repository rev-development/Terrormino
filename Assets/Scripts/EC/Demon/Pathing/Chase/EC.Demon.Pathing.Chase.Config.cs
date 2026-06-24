using System;
using UnityEngine;

namespace EC.Demon.Pathing.Chase
{
	[Serializable]
	public class Config
	{
		[field: Helpers.Editor.NavMeshAreaMaskAttribute]
		[field: SerializeField]
		public int AreaMask { get; set; } = 0;
	}
}