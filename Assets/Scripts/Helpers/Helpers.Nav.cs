using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;

namespace Helpers
{
    public static class Nav
    {

        public static void TogglePathing(NavMeshAgent navMeshAgent)
        {
            navMeshAgent.isStopped = !navMeshAgent.isStopped;

            if (navMeshAgent.isStopped)
            {
                navMeshAgent.velocity = Vector3.zero;
            }
        }

        public static void TogglePathing(NavMeshAgent navMeshAgent, bool enable)
        {
            navMeshAgent.isStopped = !enable;

            if (navMeshAgent.isStopped)
            {
                navMeshAgent.velocity = Vector3.zero;
            }
        }

        [Serializable]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
        public class AgentSteeringConfig
        {

            [SerializeField] public float Speed = 3.5f;

            [SerializeField] public float AngularSpeed = 120f;

            [SerializeField] public float Acceleration = 8f;

            [SerializeField] public float StoppingDistance = 0f;

            [SerializeField] public bool AutoBraking = true;

            public void Apply(NavMeshAgent navMeshAgent)
            {
                navMeshAgent.acceleration = Acceleration;
                navMeshAgent.angularSpeed = AngularSpeed;
                navMeshAgent.autoBraking = AutoBraking;
                navMeshAgent.speed = Speed;
                navMeshAgent.stoppingDistance = StoppingDistance;
            }

        }

    }
}