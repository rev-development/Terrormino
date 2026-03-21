using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Demon
{
    // ── Setup ──────────────────────────────────────────────────────────────
    // 1. Attach this to your Demon GameObject alongside NavMeshAgent + LightFear.
    // 2. Assign SpawnDoors  — the 3 door Transforms the demon can enter from.
    // 3. Assign PatrolPoints — waypoints around the room.
    // 4. Assign PlayerTarget — the Player Transform (or camera rig root).
    // 5. NightManager calls ApplyNightConfig() at the start of each night
    //    to set speed and patrol duration.

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(LightFear))]
    public class AI : MonoBehaviour
    {
        // ── Inspector ──────────────────────────────────────────────────────
        [Header("Patrol")]
        [Tooltip("The empty parent GameObject whose children are the patrol waypoints.")]
        public Transform PatrolPointsParent;

        [Tooltip("How long the demon idles at each patrol point before moving on.")]
        public float IdleTimeAtSpot = 3f;

        [Header("Chase")]
        [Tooltip("The player Transform the demon targets once patrol ends.")]
        public Transform PlayerTarget;

        [Tooltip("The NavMeshObstacle on the player — carving is disabled when chase starts so the demon can reach them.")]
        public NavMeshObstacle PlayerObstacle;

        [Tooltip("How fast the demon rotates to face the player while idling (degrees per second).")]
        public float LookRotationSpeed = 20f;
        [Tooltip("NavMeshAgent movement speed.")]
        public float MoveSpeed = 1.5f;

        [Tooltip("How many seconds the demon patrols before switching to chase.")]
        public float PatrolDuration = 30f;

        [Tooltip("Seconds the demon stays frozen when the flashlight hits it.")]
        public float FreezeOnLitDuration = 2f;

        // ── State machine ──────────────────────────────────────────────────
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

        // ── Unity ──────────────────────────────────────────────────────────
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = MoveSpeed;
            _agent.stoppingDistance = 0.3f;
            _lightFear = GetComponent<LightFear>();
            _lightFear.Illuminate.AddListener(OnIlluminate);
            _lightFear.Banish.AddListener(OnBanished);

            // Build patrol points array from parent's children
            // PatrolPointsParent can't be set on a prefab if it's a scene object,
            // so fall back to finding it by tag "PatrolPoints" if not assigned
            if (PatrolPointsParent == null)
            {
                var found = GameObject.FindGameObjectWithTag("PatrolPoints");
                if (found != null) PatrolPointsParent = found.transform;
            }

            if (PatrolPointsParent != null && PatrolPointsParent.childCount > 0)
            {
                _patrolPoints = new Transform[PatrolPointsParent.childCount];
                for (int i = 0; i < PatrolPointsParent.childCount; i++)
                    _patrolPoints[i] = PatrolPointsParent.GetChild(i);
            }

            // Find player references at runtime if not set on prefab
            if (PlayerTarget == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PlayerTarget = player.transform;
                    PlayerObstacle = player.GetComponent<NavMeshObstacle>();
                }
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

        // ── Phases ─────────────────────────────────────────────────────────

        private void UpdatePatrol()
        {
            _patrolTimer += Time.deltaTime;

            if (_patrolTimer >= PatrolDuration)
            {
                EnterPhase(Phase.Chasing);
                return;
            }

            if (_isMoving)
            {
                // Wait until path is ready and agent is close enough
                bool arrived = !_agent.pathPending
                    && _agent.remainingDistance <= _agent.stoppingDistance
                    && !_agent.hasPath;

                if (arrived)
                {
                    // Hard stop — kill velocity so it doesn't drift past the point
                    _agent.isStopped = true;
                    _agent.velocity = Vector3.zero;
                    _agent.updateRotation = false;
                    _isMoving = false;
                    _idleTimer = 0f;
                }
                return;
            }

            // Idling at patrol spot — slowly rotate to face player
            if (PlayerTarget != null)
            {
                Vector3 directionToPlayer = PlayerTarget.position - transform.position;
                directionToPlayer.y = 0f;

                if (directionToPlayer.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        LookRotationSpeed * Time.deltaTime
                    );
                }
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
                    // Enable carving so demon steers around player during patrol
                    if (PlayerObstacle != null) PlayerObstacle.carving = true;
                    MoveToNextPatrolPoint();
                    break;

                case Phase.Chasing:
                    // Disable carving so demon can walk straight to player
                    if (PlayerObstacle != null) PlayerObstacle.carving = false;
                    _agent.isStopped = false;
                    Debug.Log("[Demon] Switching to chase.");
                    break;
            }
        }

        private void MoveToNextPatrolPoint()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                Debug.LogWarning("[Demon.AI] MoveToNextPatrolPoint called but _patrolPoints is null or empty. Check that your patrol points parent GameObject has the 'PatrolPoints' tag.", gameObject);
                EnterPhase(Phase.Chasing);
                return;
            }

            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            _agent.updateRotation = true;
            _agent.stoppingDistance = 0.5f;
            _agent.speed = MoveSpeed;
            _agent.isStopped = false;
            _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
            _isMoving = true;
        }

        // ── Light / Freeze ─────────────────────────────────────────────────

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

        // ── Banish ─────────────────────────────────────────────────────────

        private void OnBanished(GameObject _)
        {
            if (_freezeCoroutine != null) StopCoroutine(_freezeCoroutine);
            _agent.isStopped = true;
            // Restore carving in case demon is banished during chase phase
            if (PlayerObstacle != null) PlayerObstacle.carving = true;
        }

        // ── Public API (NightManager) ──────────────────────────────────────

        public void ApplyNightConfig(float speed, float patrolDuration, float freezeDuration)
        {
            MoveSpeed = speed;
            PatrolDuration = patrolDuration;
            FreezeOnLitDuration = freezeDuration;
            if (_agent != null) _agent.speed = MoveSpeed;
        }

        // ── Helpers ────────────────────────────────────────────────────────

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