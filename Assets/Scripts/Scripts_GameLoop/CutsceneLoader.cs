using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



public class CutsceneLoader : MonoBehaviour
{
    [Tooltip("How long the cutscene plays before loading the next scene (seconds).")]
    public float CutsceneDuration = 60f;

    [Tooltip("Name of the scene to load after the cutscene finishes.")]
    public string NextSceneName = "Gameplay";

    private void Start()
    {
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(CutsceneDuration);
        SceneManager.LoadScene(NextSceneName);
    }
}