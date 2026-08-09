using System;
using Helpers.Attributes;
using UnityEngine;

namespace EC.Demon.Pathing.Chase
{
	[Serializable]
	public class ConfigData : IConfig
	{
		[field: NavMeshAreaMask] [field: SerializeField] public int AreaMask { get; set; } = 1;
	}
}