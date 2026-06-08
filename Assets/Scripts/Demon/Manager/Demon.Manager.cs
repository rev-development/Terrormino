using GameLoop;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Demon.Manager
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Demon.Manager")]
    public class Manager : Helpers.SingletonMonoBehaviour<Manager>
    {

        public List<GameObject> Demons = new();

        public GameObject DemonPrefab;

        public List<Collider> SpawnColliders = new();

        private float _graceEndTime;

        private float _nextSpawnTime;

        public Config Config { get; private set; } = new();

        public void Start()
        {
            _graceEndTime = Time.time + Config.GracePeriod;
            _nextSpawnTime = Time.time + Config.GracePeriod + Config.SpawnInterval;
        }

        public void Update()
        {
            if (Time.time >= _graceEndTime
                && Time.time >= _nextSpawnTime)
            {
                SpawnDemon();
                _nextSpawnTime = Time.time + Config.SpawnInterval;
            }
        }

        public void OnBanish(GameObject demon)
        {
            Demons.Remove(demon);
        }

        public void OnGameOver()
        {
            Demons.ForEach(Destroy);
        }

        public void SpawnDemon()
        {
            if (Demons.Count == 0)
            {
                var selectedSpawnCollider = SpawnColliders[Random.Range(0, SpawnColliders.Count)];
                Bounds spawnBounds = new(selectedSpawnCollider.bounds.center, selectedSpawnCollider.bounds.size);

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
                                out var hit,
                                10f,
                                NavMesh.AllAreas
                            ))
                    {
                        Debug.Log("[Demon.Manager] Spawning demon at " + hit.position);
                        var demon = Instantiate(DemonPrefab, hit.position, Quaternion.identity);

                        demon.GetComponent<LightFear>().Banish.AddListener(OnBanish);

                        if (demon.TryGetComponent(out AI ai))
                        {
                            ai.ApplyNightConfig(Config);
                        }

                        Demons.Add(demon);
                    }
                    else
                    {
                        Debug.LogWarning(
                                "[Demon.Manager] NavMesh.SamplePosition failed no valid NavMesh near spawn collider."
                            );
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

        public void ApplyNightConfig(NightConfig nightConfig)
        {
            Config = nightConfig.DemonConfig;
        }

    }
}