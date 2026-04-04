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
        "Expo_N5_Animation",
    };

    [Tooltip("Scene to load if no cutscene is found for the current night (e.g. title screen).")]
    public string FallbackSceneName = "TitleScreen";

    private Player.Manager _playerManager;
    private Demon.Manager _demonManager;

    public void OnJumpscare()
    {
        _demonManager.Demons.ForEach(demon => demon.SetActive(false));
        Scream.Play();
        JumpscareDemon.SetActive(true);
        Room.SetActive(false);
        StartCoroutine(EndJumpscare());
    }

    IEnumerator EndJumpscare()
    {
        yield return new WaitForSeconds(1.5f);
        Scream.Stop();
        JumpscareDemon.SetActive(false);

        // Read current night from PlayerPrefs — same key NightManager uses
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

    public void Awake()
    {
        _playerManager = Helpers.Debug.TryFindByTag("Player").GetComponent<Player.Manager>();
        if (_playerManager != null)
        {
            _playerManager.GameOver.AddListener(OnJumpscare);
        }
        else
        {
            Debug.LogWarning("[JumpscareTrigger] Could not find Player.Manager.");
        }

        _demonManager = Helpers.Debug.TryFindByTag("DemonManager").GetComponent<Demon.Manager>();
        if (_demonManager == null)
        {
            Debug.LogWarning("[JumpscareTrigger] Could not find Demon.Manager.");
        }
    }
}