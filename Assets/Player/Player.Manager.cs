using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Player
{
    public class Manager : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent GameOver = new();

        private string _sceneName;

        public void OnGameOver()
        {



            Debug.Log("Game Over");
        }

        public void BackToTitle()
        {
            SceneManager.LoadScene(_sceneName);
        }

        public void Start()
        {
            GameOver.AddListener(OnGameOver);
            if (gameObject.TryGetComponent(out ScenePicker scenePicker))
            {
                _sceneName = scenePicker.ScenePath;
            }
            else
            {
                Debug.Log($"No ScenePicker component found on {gameObject.name}", gameObject);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Demon.LightFear _))
            {
                Debug.Log(other.gameObject);
                //EditorApplication.isPaused = true;
                GameOver.Invoke();
            }
        }
    }
}
