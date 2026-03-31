using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to any persistent GameObject in the scene (e.g. the same one as NightManager).
// All scripts that need to fade lights and load a scene call FadeAndLoad() on this component.
// Each script keeps its own unique logic (dissolve, jumpscare, etc.) and delegates
// only the fade + scene load to this component.

public class SceneTransitioner : MonoBehaviour
{
    [Tooltip("Lights to fade out before loading the next scene.")]
    public List<Light> Lights = new();

    [Tooltip("Default fade duration in seconds. Can be overridden per call.")]
    public float DefaultFadeDuration = 2f;

    private Coroutine _activeTransition;

    // ── Public API ─────────────────────────────────────────────────────────

    // Fade all lights to black then load the scene.
    // Optionally pass a custom duration, otherwise uses DefaultFadeDuration.
    public void FadeAndLoad(string sceneName, float? duration = null)
    {
        if (_activeTransition != null) return; // ignore if already transitioning
        _activeTransition = StartCoroutine(FadeRoutine(sceneName, duration ?? DefaultFadeDuration));
    }

    // Fade only — useful if the calling script wants to do something after the fade
    // before loading the scene itself. Returns when fade is complete.
    public Coroutine FadeOnly(float? duration = null)
    {
        if (_activeTransition != null) return null;
        _activeTransition = StartCoroutine(FadeRoutine(null, duration ?? DefaultFadeDuration));
        return _activeTransition;
    }

    // Immediately restore all lights to their original intensity (e.g. on game over retry)
    public void RestoreLights()
    {
        if (_activeTransition != null)
        {
            StopCoroutine(_activeTransition);
            _activeTransition = null;
        }

        foreach (var light in Lights)
        {
            if (light != null && _startIntensities.ContainsKey(light))
                light.intensity = _startIntensities[light];
        }
    }

    // ── Private ────────────────────────────────────────────────────────────

    // Cache starting intensities on Awake so RestoreLights() always knows original values
    private Dictionary<Light, float> _startIntensities = new();

    private void Awake()
    {
        foreach (var light in Lights)
            if (light != null) _startIntensities[light] = light.intensity;
    }

    private IEnumerator FadeRoutine(string sceneName, float duration)
    {
        // Snapshot current intensities at the moment the fade starts
        // (in case a light was changed at runtime since Awake)
        float[] currentIntensities = new float[Lights.Count];
        for (int i = 0; i < Lights.Count; i++)
            currentIntensities[i] = Lights[i] != null ? Lights[i].intensity : 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            for (int i = 0; i < Lights.Count; i++)
                if (Lights[i] != null)
                    Lights[i].intensity = Mathf.Lerp(currentIntensities[i], 0f, t);

            yield return null;
        }

        // Ensure fully off before loading
        foreach (var light in Lights)
            if (light != null) light.intensity = 0f;

        _activeTransition = null;

        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }
}