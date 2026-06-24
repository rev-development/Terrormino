using System;
using UnityEngine;

namespace EC.Demon.Pathing.Patrol
{
	[Serializable]
	public class Config
	{
		[field: SerializeField] public float PatrolDuration { get; set; } = 40f;

		[field: SerializeField] public float IdleDuration { get; set; } = 3f;

		[field: Helpers.Editor.NavMeshAreaMaskAttribute]
		[field: SerializeField]
		public int AreaMask { get; set; } = 0;
	}
}