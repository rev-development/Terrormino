using Demon.Manager;
using System;
using UnityEngine;

namespace GameLoop
{
    [CreateAssetMenu(fileName = "NightConfig", menuName = "Terrormino/NightConfig")]
    [Serializable]
    public class NightConfig : ScriptableObject, Tetris.ITetrisConfig
    {

        public string Label;

        public int LinesRequired;

        public string CutsceneName;

        [field: SerializeField] public float Speed { get; set; }

        [field: SerializeField] public float PatrolDuration { get; set; }

        [field: SerializeField] public float FreezeDuration { get; set; }

        public Config DemonConfig;

        [field: SerializeField] public float TetrisGravityDelay { get; set; }

        [field: SerializeField] public float TetrisMoveDelay { get; set; }

        [field: SerializeField] public float TetrisLockDelay { get; set; }

    }
}