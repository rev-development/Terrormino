using System;
using Helpers.Ext;
using UnityEngine;

namespace EC.Demon
{
	[Serializable]
	public class ConfigData : IConfig
	{
		[field: SerializeField] public EC.Demon.Pathing.Patrol.ConfigSO PatrolConfigSO { get; set; }

		[field: SerializeField] public EC.Demon.Pathing.Chase.ConfigSO ChaseConfigSO { get; set; }

		[field: SerializeField] public int DemonMax { get; set; } = 1;

		[field: SerializeField] public float MaxHealth { get; set; } = 3f;

		[field: SerializeField] public float SpawnGracePeriod { get; set; } = 30f;

		[field: SerializeField] public float SpawnInterval { get; set; } = 15f;

		[field: SerializeField]
		public NavMeshAgentExt.SteeringConfig SteeringConfig { get; set; } = new()
		{
			Speed = 2f,
			AngularSpeed = 120f,
			Acceleration = 2f,
			AutoBraking = false,
			StoppingDistance = 0,
		};
	}
}