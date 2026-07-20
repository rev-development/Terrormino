using AYellowpaper.SerializedCollections;
using Tetris;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GameLoop
{
	public class NightManager : MonoBehaviour
	{
		public UnityEvent TrueGameOver = new();

		[Header("References")]
		[Tooltip("The room GameObject to disable when the night ends.")]
		public GameObject Room;

		[Tooltip("Handles light fading and scene loading.")] public SceneTransitioner SceneTransitioner;

		public SerializedDictionary<int, NightConfig> NightConfigs;

		public int NightIndex = 0;

		[Header("Events")]
		public UnityEvent<int> OnNightStarted = new(); // passes night number (1-5)

		public UnityEvent OnAllNightsWon = new();

		public static NightManager Instance { get; private set; }

		public NightConfig CurrentNight => NightConfigs[NightIndex];

		public int LinesCleared { get; private set; } = 0;

		public bool NightActive { get; private set; } = false;

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

		public void OnEnable()
		{
			TrueGameOver.AddListener(OnTrueGameOver);
			Debug.Log(Board.Instance);
			Board.Instance.LineCleared.AddListener(RegisterLineCleared);
		}

		private void Start()
		{
			var savedData = SaveManager.Instance.LoadGame();

			if (savedData != null) NightIndex = savedData.NightIndex;

			NightIndex = savedData?.NightIndex ?? 0;

			StartNight(NightIndex);
		}

		public void OnDisable() => TrueGameOver.RemoveAllListeners();

		// Called by Board.ClearLines() each time lines are cleared
		public void RegisterLineCleared(int linesJustCleared)
		{
			if (!NightActive) return;

			LinesCleared += linesJustCleared;

			Debug.Log(LinesCleared);

			if (LinesCleared >= CurrentNight.LinesRequired) WinNight();
		}

		private void StartNight(int nightIndex)
		{
			if (NightIndex >= NightConfigs.Count) return;

			LinesCleared = 0;
			NightActive = true;

			if (Demon.Manager.Manager.Instance != null) Demon.Manager.Manager.Instance.ApplyNightConfig(CurrentNight);

			if (Board.Instance != null) Board.Instance.ApplyTetrisConfig(CurrentNight);

			Debug.Log($"[NightManager] {CurrentNight.Label} started — need {CurrentNight.LinesRequired} lines.");
			OnNightStarted.Invoke(nightIndex);
		}

		private void WinNight()
		{
			NightActive = false;

			Debug.Log($"[NightManager] Night {NightIndex + 1} complete!");

			if (Demon.Manager.Manager.Instance != null) Demon.Manager.Manager.Instance.ClearAll();

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
			if (Room != null) Room.SetActive(false);

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
	}
}