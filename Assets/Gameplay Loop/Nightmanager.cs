using UnityEngine;
using UnityEngine.Events;

// Sits alongside Player.Manager in the scene.
// Wire up in Inspector:
//   - Board            → your Tetris.Board component
//   - DemonAI          → your Demon.AI component
//   - PlayerManager    → your Player.Manager component
//   - OnNightWon       → e.g. load cutscene, show "Night X Complete" UI
//   - OnAllNightsWon   → e.g. roll credits

namespace Game
{
    public class NightManager : MonoBehaviour
    {
        [Header("References")]
        public Tetris.Board Board;
        public Demon.Manager DemonManager;
        public Player.Manager PlayerManager;

        [Header("Events")]
        public UnityEvent<int> OnNightStarted = new();  // passes night number (1-5)
        public UnityEvent<int> OnNightWon = new();  // passes night number
        public UnityEvent OnAllNightsWon = new();

        // ── Night Configs ──────────────────────────────────────────────────
        // Each entry maps to Night 1–5.
        // LinesRequired    — Tetris lines to clear to win the night
        // DemonSpeed       — NavMeshAgent speed
        // DemonMaxIdle     — seconds before demon lunges without being checked
        // GravityDelay     — seconds between automatic piece drops (lower = harder)

        [System.Serializable]
        public struct NightConfig
        {
            public string Label;
            public int LinesRequired;
            public float DemonSpeed;
            public float DemonPatrolDuration; // seconds demon wanders before chasing
            public float DemonFreezeDuration; // seconds flashlight freezes demon
            public float GravityDelay;
        }

        public NightConfig[] Nights = new NightConfig[]
        {
            new NightConfig { Label = "Night 1", LinesRequired = 10, DemonSpeed = 1.0f, DemonPatrolDuration = 40f, DemonFreezeDuration = 3.0f, GravityDelay = 1.2f },
            new NightConfig { Label = "Night 2", LinesRequired = 15, DemonSpeed = 1.5f, DemonPatrolDuration = 30f, DemonFreezeDuration = 2.5f, GravityDelay = 1.0f },
            new NightConfig { Label = "Night 3", LinesRequired = 20, DemonSpeed = 2.0f, DemonPatrolDuration = 22f, DemonFreezeDuration = 2.0f, GravityDelay = 0.8f },
            new NightConfig { Label = "Night 4", LinesRequired = 25, DemonSpeed = 2.5f, DemonPatrolDuration = 15f, DemonFreezeDuration = 1.5f, GravityDelay = 0.6f },
            new NightConfig { Label = "Night 5", LinesRequired = 30, DemonSpeed = 3.2f, DemonPatrolDuration =  8f, DemonFreezeDuration = 1.0f, GravityDelay = 0.4f },
        };

        // ── State ──────────────────────────────────────────────────────────
        public int CurrentNight { get; private set; } = 0; // 0-indexed internally
        public int LinesCleared { get; private set; } = 0;
        public bool NightActive { get; private set; } = false;

        // ── Unity ──────────────────────────────────────────────────────────
        private void Start()
        {
            if (PlayerManager != null)
                PlayerManager.GameOver.AddListener(OnGameOver);

            StartNight(0);
        }

        // ── Public API ─────────────────────────────────────────────────────

        // Call this from your Board when a line is cleared.
        // Hook it up in the Inspector to Board's line-clear event,
        // or call it directly from Board.ClearLines().
        public void RegisterLineCleared(int linesJustCleared)
        {
            if (!NightActive) return;

            LinesCleared += linesJustCleared;

            int required = Nights[CurrentNight].LinesRequired;
            if (LinesCleared >= required)
            {
                WinNight();
            }
        }

        // ── Private ────────────────────────────────────────────────────────
        private void StartNight(int nightIndex)
        {
            if (nightIndex >= Nights.Length) return;

            CurrentNight = nightIndex;
            LinesCleared = 0;
            NightActive = true;

            NightConfig cfg = Nights[nightIndex];

            // Apply demon config
            if (DemonManager != null)
                DemonManager.ApplyNightConfig(cfg.DemonSpeed, cfg.DemonPatrolDuration, cfg.DemonFreezeDuration);

            // Apply Tetris gravity
            if (Board != null)
                Board.Config.GravityDelay = cfg.GravityDelay;

            Debug.Log($"[NightManager] {cfg.Label} started — need {cfg.LinesRequired} lines.");
            OnNightStarted.Invoke(nightIndex + 1);
        }

        private void WinNight()
        {
            NightActive = false;
            int humanNight = CurrentNight + 1;

            Debug.Log($"[NightManager] Night {humanNight} complete!");
            OnNightWon.Invoke(humanNight);

            if (CurrentNight + 1 >= Nights.Length)
            {
                Debug.Log("[NightManager] All nights survived — you win!");
                OnAllNightsWon.Invoke();
            }
            else
            {

                AdvanceToNextNight(); //temp testing
            }
        }

        //Used for starting the next night when needed
        public void AdvanceToNextNight()
        {
            StartNight(CurrentNight + 1);
        }

        private void OnGameOver()
        {
            NightActive = false;
            Debug.Log($"[NightManager] Game over on Night {CurrentNight + 1}.");
        }
    }
}