using System;
using UnityEngine;

namespace Demon.Manager
{
    [Serializable]
    public class Config : IConfigDto
    {

        [field: SerializeField] public float MaxHealth { get; set; } = 3f;

        [field: SerializeField] public float MoveSpeed { get; set; } = 1f;

        [field: SerializeField] public float PatrolDuration { get; set; } = 40f;

        [field: SerializeField] public float FreezeDuration { get; set; } = 3f;

        [field: SerializeField] public float GracePeriod { get; set; } = 30f;

        [field: SerializeField] public float SpawnInterval { get; set; } = 15f;

    }
}