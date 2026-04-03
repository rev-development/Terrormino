using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Sits alongside Player.Manager in the scene.
// Wire up in Inspector:
//   - Board            → your Tetris.Board component
//   - DemonManager     → your Demon.Manager component
//   - PlayerManager    → your Player.Manager component
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
        public UnityEvent OnAllNightsWon = new();

        [Header("Transition")]
        [Tooltip("The room GameObject to disable when the night ends.")]
        public GameObject Room;

        [Tooltip("Handles light fading and scene loading.")]
        public SceneTransitioner SceneTransitioner;

        [Tooltip("How long the fade to black takes in seconds.")]
        public float FadeDuration = 2f;
        [Tooltip("Cutscene scene names in order — index 0 = after night 1, index 1 = after night 2, etc.")]
        public string[] CutsceneSceneNames = new string[]
        {
            "Cutscene1",
            "Cutscene2",
            "Cutscene3",
            "Cutscene4",
            // No cutscene after night 5 — use OnAllNightsWon instead
        };

        // ── Night Configs ──────────────────────────────────────────────────
        [System.Serializable]
        public struct NightConfig
        {
            public string Label;
            public int LinesRequired;
            public float DemonSpeed;
            public float DemonPatrolDuration;
            public float DemonFreezeDuration;
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
        public int CurrentNight { get; private set; } = 0;
        public int LinesCleared { get; private set; } = 0;
        public bool NightActive { get; private set; } = false;

        // ── Unity ──────────────────────────────────────────────────────────
        private void Start()
        {
            if (PlayerManager != null)
                PlayerManager.GameOver.AddListener(OnGameOver);

            // Read which night to start from PlayerPrefs so cutscene
            // scenes can hand back to gameplay at the correct night
            int savedNight = PlayerPrefs.GetInt("CurrentNight", 0);
            StartNight(savedNight);
        }

        // ── Public API ─────────────────────────────────────────────────────

        // Called by Board.ClearLines() each time lines are cleared
        public void RegisterLineCleared(int linesJustCleared)
        {
            if (!NightActive) return;

            LinesCleared += linesJustCleared;

            if (LinesCleared >= Nights[CurrentNight].LinesRequired)
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

            if (DemonManager != null)
                DemonManager.ApplyNightConfig(cfg.DemonSpeed, cfg.DemonPatrolDuration, cfg.DemonFreezeDuration);

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

            if (DemonManager != null)
                DemonManager.ClearAll();

            bool isLastNight = CurrentNight + 1 >= Nights.Length;

            if (isLastNight)
            {
                PlayerPrefs.DeleteKey("CurrentNight");
                PlayerPrefs.Save();
                Debug.Log("[NightManager] All nights survived — you win!");
                OnAllNightsWon.Invoke();
            }
            else
            {
                PlayerPrefs.SetInt("CurrentNight", CurrentNight + 1);
                PlayerPrefs.Save();

                if (CurrentNight < CutsceneSceneNames.Length)
                    StartCutsceneTransition(CutsceneSceneNames[CurrentNight]);
                else
                {
                    Debug.LogWarning($"[NightManager] No cutscene defined for night {humanNight}, skipping.");
                    StartNight(CurrentNight + 1);
                }
            }
        }

        private void StartCutsceneTransition(string sceneName)
        {
            if (Room != null) Room.SetActive(false);

            if (SceneTransitioner != null)
                SceneTransitioner.FadeAndLoad(sceneName, FadeDuration);
            else
            {
                Debug.LogWarning("[NightManager] No SceneTransitioner assigned — loading scene immediately.");
                SceneManager.LoadScene(sceneName);
            }
        }

        private void OnGameOver()
        {
            NightActive = false;
            // Clear saved progress on game over so next run starts from night 1
            PlayerPrefs.DeleteKey("CurrentNight");
            PlayerPrefs.Save();
            Debug.Log($"[NightManager] Game over on Night {CurrentNight + 1}.");
        }
    }
}