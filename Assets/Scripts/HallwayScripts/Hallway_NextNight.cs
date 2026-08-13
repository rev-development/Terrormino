using UnityEngine;
using UnityEngine.SceneManagement;

public class Hallway_NextNight : MonoBehaviour {
    private string _sceneName;

    private void Start() {
        if (TryGetComponent(out Helpers.ScenePicker scenePicker)) {
            Debug.Log($"[Hallway_NextNight] Scene loaded: {_sceneName}");
            _sceneName = scenePicker.ScenePath;
        } else {
            Debug.LogWarning($"[Hallway_NextNight] No ScenePicker component found on {gameObject.name}.", gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponentInParent<Player.Manager>() != null) {
            if (!string.IsNullOrEmpty(_sceneName))
                SceneManager.LoadScene(_sceneName);
            else
                Debug.LogError("[Hallway_NextNight] No scene assigned.", gameObject);
        }
    }
}