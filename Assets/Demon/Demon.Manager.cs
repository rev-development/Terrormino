using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Demon
{
    public class Manager : MonoBehaviour
    {
        public List<GameObject> Demons = new();
        public GameObject DemonPrefab;
        public List<Collider> SpawnColliders = new();

        [Header("Timing")]
        [Tooltip("Seconds before the first demon can spawn.")]
        public float GracePeriod = 30f;

        [Tooltip("Seconds between spawn attempts after grace period ends.")]
        public float SpawnInterval = 15f;

        private float _graceEndTime;
        private float _nextSpawnTime;

        // Set by NightManager after each night starts so newly spawned
        // demons immediately get the correct difficulty config
        private float _nightSpeed = 1.0f;
        private float _nightPatrolDuration = 40f;
        private float _nightFreezeDuration = 3.0f;

        public void Start()
        {
            _graceEndTime = Time.time + GracePeriod;
            _nextSpawnTime = Time.time + GracePeriod + SpawnInterval;
        }

        // Called by NightManager at the start of each night
        public void ApplyNightConfig(float speed, float patrolDuration, float freezeDuration)
        {
            _nightSpeed = speed;
            _nightPatrolDuration = patrolDuration;
            _nightFreezeDuration = freezeDuration;

            // Apply to any demon already alive (e.g. carried over mid-night)
            Demons.ForEach(d =>
            {
                if (d != null && d.TryGetComponent(out AI ai))
                    ai.ApplyNightConfig(_nightSpeed, _nightPatrolDuration, _nightFreezeDuration);
            });
        }

        public void OnBanish(GameObject demon)
        {
            Demons.Remove(demon);
        }

        public void SpawnDemon()
        {
            if (Demons.Count == 0)
            {
                var selectedSpawnCollider = SpawnColliders[Random.Range(0, SpawnColliders.Count)];
                Bounds spawnBounds = new(
                    selectedSpawnCollider.bounds.center,
                    selectedSpawnCollider.bounds.size
                );

                if (DemonPrefab.TryGetComponent(out Collider demonCollider))
                {
                    spawnBounds.Expand(-demonCollider.bounds.extents);
                    Vector3 spawnLocation = new(
                        Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                        spawnBounds.center.y,
                        Random.Range(spawnBounds.min.z, spawnBounds.max.z)
                    );

                    if (NavMesh.SamplePosition(
                            spawnLocation,
                            out NavMeshHit hit,
                            10f,
                            NavMesh.AllAreas
                        ))
                    {
                        Debug.Log("[Demon.Manager] Spawning demon at " + hit.position);
                        var demon = Instantiate(DemonPrefab, hit.position, Quaternion.identity);

                        demon.GetComponent<LightFear>().Banish.AddListener(OnBanish);

                        if (demon.TryGetComponent(out AI ai))
                            ai.ApplyNightConfig(_nightSpeed, _nightPatrolDuration, _nightFreezeDuration);

                        Demons.Add(demon);
                    }
                    else
                    {
                        Debug.LogWarning("[Demon.Manager] NavMesh.SamplePosition failed — no valid NavMesh near spawn collider.");
                    }
                }
                else
                {
                    Debug.LogWarning("[Demon.Manager] DemonPrefab has no Collider component.");
                }
            }
        }

        public void ClearAll()
        {
            Demons.ForEach(demon => demon.GetComponent<LightFear>().Banish.Invoke(demon));
        }

        public void Update()
        {
            if (Time.time >= _graceEndTime && Time.time >= _nextSpawnTime)
            {
                SpawnDemon();
                _nextSpawnTime = Time.time + SpawnInterval;
            }
        }
    }
}