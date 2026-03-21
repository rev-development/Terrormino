using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Demon
{


    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(LightFear))]
    public class AI : MonoBehaviour
    {

        [Header("Patrol")]
        [Tooltip("The empty parent GameObject whose children are the patrol waypoints.")]
        public Transform PatrolPointsParent;

        [Tooltip("How long the demon idles at each patrol point before moving on.")]
        public float IdleTimeAtSpot = 3f;

        [Header("Chase")]
        [Tooltip("The player Transform the demon targets once patrol ends.")]
        public Transform PlayerTarget;

        [Header("Night Config (set by NightManager)")]
        [Tooltip("NavMeshAgent movement speed.")]
        public float MoveSpeed = 1.5f;

        [Tooltip("How many seconds the demon patrols before switching to chase.")]
        public float PatrolDuration = 30f;

        [Tooltip("Seconds the demon stays frozen when the flashlight hits it.")]
        public float FreezeOnLitDuration = 2f;


        private enum Phase { Patrolling, Chasing, Frozen }
        private Phase _phase = Phase.Patrolling;
        private Phase _phaseBeforeFreeze;

        private NavMeshAgent _agent;
        private LightFear _lightFear;

        private Transform[] _patrolPoints;
        private int _currentPatrolIndex = 0;
        private float _idleTimer = 0f;
        private float _patrolTimer = 0f;
        private bool _isMoving = false;
        private bool _isLit = false;

        private Coroutine _freezeCoroutine;


        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = MoveSpeed;
            _agent.stoppingDistance = 0.3f;

            _lightFear = GetComponent<LightFear>();
            _lightFear.Illuminate.AddListener(OnIlluminate);
            _lightFear.Banish.AddListener(OnBanished);

            // Build patrol points array from parent's children
            if (PatrolPointsParent != null && PatrolPointsParent.childCount > 0)
            {
                _patrolPoints = new Transform[PatrolPointsParent.childCount];
                for (int i = 0; i < PatrolPointsParent.childCount; i++)
                    _patrolPoints[i] = PatrolPointsParent.GetChild(i);
            }

            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                Debug.LogWarning("Demon.AI: No PatrolPoints found, going straight to chase.", gameObject);
                EnterPhase(Phase.Chasing);
                return;
            }

            ShufflePatrolPoints();
            EnterPhase(Phase.Patrolling);
        }

        private void Update()
        {
            switch (_phase)
            {
                case Phase.Patrolling: UpdatePatrol(); break;
                case Phase.Chasing: UpdateChase(); break;
            }
        }



        private void UpdatePatrol()
        {
            _agent.speed = MoveSpeed;
            _patrolTimer += Time.deltaTime;

            // Switch to chase once patrol time is up
            if (_patrolTimer >= PatrolDuration)
            {
                EnterPhase(Phase.Chasing);
                return;
            }

            if (_isMoving)
            {
                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _isMoving = false;
                    _idleTimer = 0f;
                }
                return;
            }

            _idleTimer += Time.deltaTime;

            if (_idleTimer >= IdleTimeAtSpot)
            {
                MoveToNextPatrolPoint();
            }
        }

        private void UpdateChase()
        {
            if (PlayerTarget == null) return;
            _agent.speed = MoveSpeed * 1.2f; // slightly faster when chasing
            _agent.SetDestination(PlayerTarget.position);
        }

        private void EnterPhase(Phase newPhase)
        {
            _phase = newPhase;

            switch (newPhase)
            {
                case Phase.Patrolling:
                    MoveToNextPatrolPoint();
                    break;

                case Phase.Chasing:
                    _agent.isStopped = false;
                    Debug.Log("[Demon] Switching to chase.");
                    break;
            }
        }

        private void MoveToNextPatrolPoint()
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            _agent.isStopped = false;
            _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
            _isMoving = true;
        }



        private void OnIlluminate(bool illuminated)
        {
            _isLit = illuminated;

            if (illuminated && _phase != Phase.Frozen)
            {
                if (_freezeCoroutine != null) StopCoroutine(_freezeCoroutine);
                _freezeCoroutine = StartCoroutine(FreezeRoutine());
            }
        }

        private IEnumerator FreezeRoutine()
        {
            _phaseBeforeFreeze = _phase;
            _phase = Phase.Frozen;
            _agent.isStopped = true;

            yield return new WaitForSeconds(FreezeOnLitDuration);

            // Keep frozen while still lit
            while (_isLit)
            {
                yield return new WaitForSeconds(0.5f);
            }

            // Resume whatever phase we were in
            _phase = _phaseBeforeFreeze;
            _agent.isStopped = false;
            _freezeCoroutine = null;
        }



        private void OnBanished(GameObject _)
        {
            if (_freezeCoroutine != null) StopCoroutine(_freezeCoroutine);
            _agent.isStopped = true;
        }

        // ── Public API (NightManager) ──────────────────────────────────────

        public void ApplyNightConfig(float speed, float patrolDuration, float freezeDuration)
        {
            MoveSpeed = speed;
            PatrolDuration = patrolDuration;
            FreezeOnLitDuration = freezeDuration;
            if (_agent != null) _agent.speed = MoveSpeed;
        }



        private void ShufflePatrolPoints()
        {
            for (int i = _patrolPoints.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_patrolPoints[i], _patrolPoints[j]) = (_patrolPoints[j], _patrolPoints[i]);
            }
        }
    }
}