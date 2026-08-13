using System;
using UnityEngine;

namespace GameLoop
{
    [CreateAssetMenu(fileName = "NightConfig", menuName = "Terrormino/NightConfig")]
    [Serializable]
    public class NightConfig : ScriptableObject, Tetris.ITetrisConfig, Demon.Manager.IDemonConfig
    {

        public string Label;
        public int LinesRequired;
        public string CutsceneName;

        [field: SerializeField] public float DemonSpeed { get; set; }
        [field: SerializeField] public float DemonPatrolDuration { get; set; }
        [field: SerializeField] public float DemonFreezeDuration { get; set; }

        [field: SerializeField] public float TetrisGravityDelay { get; set; }
        [field: SerializeField] public float TetrisMoveDelay { get; set; }
        [field: SerializeField] public float TetrisLockDelay { get; set; }

    }
}