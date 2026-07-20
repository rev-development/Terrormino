using System;
using UnityEngine;

namespace EC.Demon
{
	[Serializable]
	public class Config : IConfigDto
	{
		[field: SerializeField] public Pathing.Chase.Config ChaseConfig { get; set; } = new();

		[field: SerializeField] public int DemonMax { get; set; } = 1;

		[field: SerializeField] public Pathing.Patrol.Config PatrolConfig { get; set; } = new();

		[field: SerializeField]
		public Helpers.Ext.NavMeshAgentExt.SteeringConfig SteeringConfig { get; set; } = new()
			{
				Speed = 2f,
				AngularSpeed = 120f,
				Acceleration = 2f,
				AutoBraking = false,
				StoppingDistance = 0,
			};

		[field: SerializeField] public float MaxHealth { get; set; } = 3f;

		[field: SerializeField] public float SpawnGracePeriod { get; set; } = 30f;

		[field: SerializeField] public float SpawnInterval { get; set; } = 15f;
	}
}