using GameLoop;
using Tetris;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Player
{
	public class Manager : MonoBehaviour
	{
		public UnityEvent GameOver = new();

		private string _sceneName;

		public static Manager Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null
				&& Instance != this)
			{
				Destroy(gameObject);

				return;
			}

			Instance = this;
		}

		public void OnEnable() => Board.Instance.TetrisLose.AddListener(OnGameOver);

		public void Start()
		{
			if (gameObject.TryGetComponent(out Helpers.ScenePicker scenePicker))
				_sceneName = scenePicker.ScenePath;
			else
				Debug.Log($"No ScenePicker component found on {gameObject.name}", gameObject);
		}

		public void OnDisable() => GameOver.RemoveAllListeners();

		public void BackToTitle() => SceneManager.LoadScene(_sceneName);

		/// <summary>
		///     This is triggered by the demon and Tetris
		///     NOTE: This should only explicitly handle events within the current scene, anything related to persistent data and
		///     scene transitions should occur in the NightManager
		/// </summary>
		public void OnGameOver()
		{
			Debug.Log("Game Over");
			NightManager.Instance.TrueGameOver.Invoke();
		}
	}
}