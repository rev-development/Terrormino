using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GameLoop
{
    public class NightManager : MonoBehaviour
    {

        // Called by Board.ClearLines() each time lines are cleared
        public void RegisterLineCleared(int linesJustCleared)
        {
            Debug.Log(linesJustCleared);
            if (!NightActive)
            {
                Debug.Log("Night Not Active");
                return;
            }

            LinesCleared += linesJustCleared;

            Debug.Log(LinesCleared);

            if (LinesCleared >= CurrentNight.LinesRequired)
            {
                WinNight();
            }
        }

        private void StartNight(int nightIndex)
        {
            if (NightIndex >= NightConfigs.Count)
            {
                return;
            }

            LinesCleared = 0;
            NightActive = true;


            if (Demon.Manager.Instance != null)
            {
                Demon.Manager.Instance.ApplyNightConfig(CurrentNight);
            }

            if (Tetris.Board.Instance != null)
            {
                Tetris.Board.Instance.ApplyTetrisConfig(CurrentNight);
            }

            Debug.Log($"[NightManager] {CurrentNight.Label} started — need {CurrentNight.LinesRequired} lines.");
            OnNightStarted.Invoke(nightIndex);
        }

        private void WinNight()
        {
            NightActive = false;

            Debug.Log($"[NightManager] Night {NightIndex + 1} complete!");

            if (Demon.Manager.Instance != null)
            {
                Demon.Manager.Instance.ClearAll();
            }

            NightIndex += 1;

            if (NightIndex + 1 >= NightConfigs.Count)
            {
                SaveManager.Instance.SaveGame(new SaveData(NightIndex));
                Debug.Log("[NightManager] All nights survived — you win!");
                OnAllNightsWon.Invoke();
            }
            else
            {
                SaveManager.Instance.SaveGame(new SaveData(NightIndex));

                if (!string.IsNullOrEmpty(CurrentNight.CutsceneName))
                {
                    StartCutsceneTransition(CurrentNight.CutsceneName);
                }
                else
                {
                    Debug.LogWarning($"[NightManager] No cutscene defined for night {NightIndex}, skipping.");
                    StartNight(NightIndex);
                }
            }
        }

        private void StartCutsceneTransition(string sceneName)
        {
            if (Room != null)
            {
                Room.SetActive(false);
            }

            if (SceneTransitioner != null)
            {
                SceneTransitioner.FadeAndLoad(sceneName);
            }
            else
            {
                Debug.LogWarning("[NightManager] No SceneTransitioner assigned — loading scene immediately.");
                SceneManager.LoadScene(sceneName);
            }
        }

        private void OnTrueGameOver()
        {
            NightActive = false;

            Debug.Log($"[NightManager] Game over on Night {NightIndex}.");
        }

        #region References

        [Header("References")]
        [Tooltip("The room GameObject to disable when the night ends.")]
        public GameObject Room;
        [Tooltip("Handles light fading and scene loading.")]
        public SceneTransitioner SceneTransitioner;
        public SerializedDictionary<int, NightConfig> NightConfigs;

        public static NightManager Instance { get; private set; }

        #endregion

        #region Runtime Values

        public int NightIndex = 0;
        public NightConfig CurrentNight => NightConfigs[NightIndex];
        public int LinesCleared { get; private set; } = 0;

        public bool NightActive { get; private set; } = false;

        #endregion

        #region Events

        [Header("Events")]
        public UnityEvent<int> OnNightStarted = new(); // passes night number (1-5)
        public UnityEvent OnAllNightsWon = new();

        #endregion

        #region Lifecycle

        private void Awake()
        {
            if (Instance != null
                && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;

            DontDestroyOnLoad(this);
            
        }

        private void Start()
        {
            var savedData = SaveManager.Instance.LoadGame();

            if (savedData != null)
            {
                NightIndex = savedData.NightIndex;
            }

            NightIndex = savedData?.NightIndex ?? 0;

            StartNight(NightIndex);
        }

        public void OnEnable()
        {
            Player.Manager.Instance.TrueGameOver.AddListener(OnTrueGameOver);
            Tetris.Board.Instance.LineCleared.AddListener(RegisterLineCleared);
        }

        #endregion

    }
}