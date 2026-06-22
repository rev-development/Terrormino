using System.Collections.Generic;
using UnityEngine;

public class HallwaySfx : MonoBehaviour
{
    public List<AudioSource> HallwaySFXSources = new();
    private SFXTrigger _sfxTrigger;

    void Start()
    {
        var triggerObject = GameObject.Find("HallwaySoundTriggers");
        if (triggerObject != null)
        {
            _sfxTrigger = triggerObject.GetComponent<SFXTrigger>();
        }

        if (_sfxTrigger != null)
        {
            Debug.Log("[HallwaySfx] SFXTrigger found, listener attached.");
            _sfxTrigger.hallwayTriggerEvent.AddListener(PlayRandomHallwaySFX);
        }
        else
        {
            Debug.LogWarning("[HallwaySfx] Could not find SFXTrigger on 'HallwaySoundTriggers'.");
        }

        ConnectHallwaySFX();
        Debug.Log($"[HallwaySfx] Connected {HallwaySFXSources.Count} audio sources.");
    }

    private void ConnectHallwaySFX()
    {
        var parent = GameObject.Find("HallwayAudioSFX");
        if (parent == null)
        {
            Debug.LogWarning("[HallwaySfx] Could not find 'HallwayAudioSFX' GameObject in scene.");
            return;
        }

        HallwaySFXSources.Clear();

        // Each child (SFX1, SFX2, ...) has its own AudioSource at its own position
        foreach (Transform child in parent.transform)
        {
            var source = child.GetComponent<AudioSource>();
            if (source != null)
                HallwaySFXSources.Add(source);
        }

        if (HallwaySFXSources.Count == 0)
            Debug.LogWarning("[HallwaySfx] No AudioSource components found under 'HallwayAudioSFX'.");
    }


    private void PlayRandomHallwaySFX()
    {
        Debug.Log($"[HallwaySfx] PlayRandomHallwaySFX called. Source count: {HallwaySFXSources?.Count ?? 0}");

        if (HallwaySFXSources == null || HallwaySFXSources.Count == 0) return;

        var sound = HallwaySFXSources[Random.Range(0, HallwaySFXSources.Count)];
        Debug.Log($"[HallwaySfx] Playing: {sound?.gameObject.name}, clip assigned: {sound?.clip != null}");
        if (sound != null) sound.Play();
    }

    // TODO: implement sequential playback (e.g. footsteps moving down the hallway)

}