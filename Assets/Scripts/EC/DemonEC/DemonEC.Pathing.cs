using UnityEngine;
using UnityEngine.AI;

namespace EC.DemonEC
{
    public class Pathing : MonoBehaviour
    {

        public GameObject PatrolPointsParent;

        public GameObject PlayerTarget;

        private int _currentPatrolIndex = 0;

        private NavMeshAgent _navMeshAgent;

        public class Config
        {

            public float FreezeOnLitDuration = 2f;

            public float IdleTimeAtSpot = 3f;

            public float LookRotationSpeed = 20f;

            public float MoveSpeed = 1.5f;

            public float PatrolDuration = 30f;

        }

    }
}