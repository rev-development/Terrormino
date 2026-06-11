using UnityEngine;
using UnityEngine.AI;

namespace Helpers
{
    public static class NavMesh
    {

        public static void FullStop(NavMeshAgent navMeshAgent)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.ResetPath();
        }

        public static void FullStop(NavMeshAgent navMeshAgent, Rigidbody rb)
        {
            FullStop(navMeshAgent);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }
}