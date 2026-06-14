using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;

namespace Helpers
{
    public static class Nav
    {

        /// <summary>
        ///     Sets isStopped to either opposite val then sets velocity to 0 if isStopped.
        /// </summary>
        /// <param name="navMeshAgent"></param>
        public static void TogglePathing(NavMeshAgent navMeshAgent) {
            navMeshAgent.isStopped = !navMeshAgent.isStopped;

            if (navMeshAgent.isStopped) navMeshAgent.velocity = Vector3.zero;
        }

        /// <summary>
        ///     Sets is isStopped to the OPPOSITE value passed through.
        ///     True is Green Light, False is Red Light
        /// </summary>
        /// <param name="navMeshAgent"></param>
        /// <param name="pathingEnabled"></param>
        public static void TogglePathing(NavMeshAgent navMeshAgent, bool pathingEnabled) {
            navMeshAgent.isStopped = !pathingEnabled;

            if (navMeshAgent.isStopped) navMeshAgent.velocity = Vector3.zero;
        }

        [Serializable]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
        public class AgentSteeringConfig
        {

            // All values are default values for NavMeshAgent

            [SerializeField] public float Speed = 3.5f;

            [SerializeField] public float AngularSpeed = 120f;

            [SerializeField] public float Acceleration = 8f;

            [SerializeField] public float StoppingDistance = 0f;

            [SerializeField] public bool AutoBraking = true;

            public void Apply(NavMeshAgent navMeshAgent) {
                navMeshAgent.acceleration = Acceleration;
                navMeshAgent.angularSpeed = AngularSpeed;
                navMeshAgent.autoBraking = AutoBraking;
                navMeshAgent.speed = Speed;
                navMeshAgent.stoppingDistance = StoppingDistance;
            }

        }

    }
}