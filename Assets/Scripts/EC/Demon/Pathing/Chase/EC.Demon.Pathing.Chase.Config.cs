using System;
using UnityEngine;

namespace EC.Demon.Pathing.Chase
{
	[Serializable]
	public class Config
	{
		[field: Helpers.Attributes.NavMeshAreaMaskAttribute]
		[field: SerializeField]
		public int AreaMask { get; set; } = 0;
	}
}