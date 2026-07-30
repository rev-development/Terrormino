using System;
using Helpers.Attributes;
using UnityEngine;

namespace EC.Demon.Pathing.Patrol
{
	[Serializable]
	public class ConfigData : IConfig
	{
		[field: NavMeshAreaMask] [field: SerializeField] public int AreaMask { get; set; } = 1;

		[field: SerializeField] public float PatrolDuration { get; set; } = 40f;

		[field: SerializeField] public float IdleDuration { get; set; } = 3f;
	}
}