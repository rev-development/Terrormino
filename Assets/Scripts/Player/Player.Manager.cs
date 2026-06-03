using Tetris;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Player
{
    public class Manager : MonoBehaviour
    {

        public UnityEvent GameOver = new();

        public UnityEvent TrueGameOver = new();

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

        public void Start()
        {
            if (gameObject.TryGetComponent(out Helpers.ScenePicker scenePicker))
            {
                _sceneName = scenePicker.ScenePath;
            }
            else
            {
                Debug.Log($"No ScenePicker component found on {gameObject.name}", gameObject);
            }
        }

        public void OnEnable()
        {
            Board.Instance.TetrisLose.AddListener(OnGameOver);
        }

        public void OnDisable()
        {
            GameOver.RemoveAllListeners();
            TrueGameOver.RemoveAllListeners();
        }

        /// <summary>
        ///     This is triggered by the demon and Tetris
        ///     NOTE: This should only explicitly handle events within the current scene, anything related to persistent data and
        ///     scene transitions should occur in the NightManager
        /// </summary>
        public void OnGameOver()
        {
            Debug.Log("Game Over");
            TrueGameOver.Invoke();
        }

        public void BackToTitle()
        {
            SceneManager.LoadScene(_sceneName);
        }

    }
}