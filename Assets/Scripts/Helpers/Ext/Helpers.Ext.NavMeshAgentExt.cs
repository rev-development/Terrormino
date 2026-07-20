using System;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable MemberCanBePrivate.Global

namespace Helpers.Ext
{
	public static class NavMeshAgentExt
	{
		/// <summary>
		///     Sets isStopped to either opposite val then sets velocity to 0 if isStopped.
		/// </summary>
		/// <param name="navMeshAgent"></param>
		public static void TogglePathing(this NavMeshAgent navMeshAgent)
		{
			navMeshAgent.isStopped = !navMeshAgent.isStopped;

			if (navMeshAgent.isStopped) navMeshAgent.velocity = Vector3.zero;
		}

		public static void StopResetDisable(this NavMeshAgent navMeshAgent)
		{
			navMeshAgent.TogglePathing(false);
			navMeshAgent.ResetPath();
			navMeshAgent.enabled = false;
		}

		/// <summary>
		///     Sets is isStopped to the OPPOSITE value passed through.
		///     True is Green Light, False is Red Light
		/// </summary>
		/// <param name="navMeshAgent"></param>
		/// <param name="pathingEnabled"></param>
		public static void TogglePathing(this NavMeshAgent navMeshAgent, bool pathingEnabled)
		{
			navMeshAgent.isStopped = !pathingEnabled;

			if (navMeshAgent.isStopped) navMeshAgent.velocity = Vector3.zero;
		}

		public static void ApplyAreaMask(this NavMeshAgent navMeshAgent, int areaMask) =>
			navMeshAgent.areaMask = areaMask;

		public static bool IsAtDestination(this NavMeshAgent navMeshAgent) =>
			!navMeshAgent.pathPending
			&& navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
			&& !navMeshAgent.hasPath;

		public static void GoTo(this NavMeshAgent navMeshAgent, Vector3 destination)
		{
			navMeshAgent.TogglePathing(true);
			navMeshAgent.SetDestination(destination);
		}

		public static void GoTo(this NavMeshAgent navMeshAgent, GameObject destinationGameObject)
		{
			navMeshAgent.TogglePathing(true);
			navMeshAgent.SetDestination(destinationGameObject.transform.position);
		}

		public static void ApplySteeringConfig(this NavMeshAgent navMeshAgent, SteeringConfig steeringConfig)
		{
			navMeshAgent.acceleration = steeringConfig.Acceleration;
			navMeshAgent.angularSpeed = steeringConfig.AngularSpeed;
			navMeshAgent.autoBraking = steeringConfig.AutoBraking;
			navMeshAgent.speed = steeringConfig.Speed;
			navMeshAgent.stoppingDistance = steeringConfig.StoppingDistance;
		}

		public interface ISteeringConfig
		{
			public float Speed { get; }

			public float AngularSpeed { get; }

			public float Acceleration { get; }

			public float StoppingDistance { get; }

			public bool AutoBraking { get; }
		}

		[Serializable]
		public class SteeringConfig : ISteeringConfig
		{
			// All values are default values for NavMeshAgent

			[field: SerializeField] public float Speed { get; set; } = 3.5f;

			[field: SerializeField] public float AngularSpeed { get; set; } = 120f;

			[field: SerializeField] public float Acceleration { get; set; } = 8f;

			[field: SerializeField] public float StoppingDistance { get; set; } = 0f;

			[field: SerializeField] public bool AutoBraking { get; set; } = true;
		}
	}
}