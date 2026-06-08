using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class JumpscareTrigger : MonoBehaviour
{

    public AudioSource Scream;

    public GameObject JumpscareDemon;

    public GameObject Room;

    public Light MoonLight;

    public UnityEvent Jumpscare;

    [Tooltip("Must match the CutsceneSceneNames array in NightManager exactly.")]
    public List<string> CutsceneSceneNames = new()
    {
        "Expo_N1_Animation",
        "Expo_N2_Animation",
        "Expo_N3_Animation",
        "Expo_N4_Animation",
        "Expo_N5_Animation"
    };

    [Tooltip("Scene to load if no cutscene is found for the current night (e.g. title screen).")]
    public string FallbackSceneName = "TitleScreen";

    public void OnJumpscare()
    {
        Scream.Play();
        JumpscareDemon.SetActive(true);
        Room.SetActive(false);
        StartCoroutine(EndJumpscare());
    }

    private IEnumerator EndJumpscare()
    {
        yield return new WaitForSeconds(1.5f);

        Scream.Stop();
        JumpscareDemon.SetActive(false);

        // Read current night from PlayerPrefs � same key NightManager uses
        int currentNight = PlayerPrefs.GetInt("CurrentNight", 0);

        string sceneToLoad;

        if (currentNight < CutsceneSceneNames.Count)
        {
            sceneToLoad = CutsceneSceneNames[currentNight];
        }
        else
        {
            Debug.LogWarning($"[JumpscareTrigger] No cutscene for night {currentNight}, loading fallback.");
            sceneToLoad = FallbackSceneName;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

}