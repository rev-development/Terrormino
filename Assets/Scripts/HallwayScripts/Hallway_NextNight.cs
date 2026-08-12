using UnityEngine;
using UnityEngine.SceneManagement;

public class Hallway_NextNight : MonoBehaviour {
    private string _sceneName;

    private void Start() {
        if (TryGetComponent(out Helpers.ScenePicker scenePicker)) {
            _sceneName = scenePicker.ScenePath;
        } else {
            Debug.LogWarning($"[Hallway_NextNight] No ScenePicker component found on {gameObject.name}.", gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!string.IsNullOrEmpty(_sceneName))
                SceneManager.LoadScene(_sceneName);
            else
                Debug.LogError("[Hallway_NextNight] No scene assigned -- add a ScenePicker component and drag in a scene.", gameObject);
        }
    }
}