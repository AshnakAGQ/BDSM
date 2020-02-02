﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // Storage of all AudioClip objects ready to be played
    private Dictionary<string, AudioClip> clipStorage = new Dictionary<string, AudioClip>();

    // The number of current live AudioSource objects created by this script
    private int activeSources;

    // The maximum number of AudioSource objects allowed to be live at once
    private static int maxSources = 20;

    private void Awake()
    {
        activeSources = 0;
    }

    // takes a string filename in the path Resources/Audio/SFX
    // loads that audio file into the AudioPlayer's storage
    public void LoadClip(string name)
    {
        if (clipStorage.ContainsKey(name))
        {
            return;
        }
        AudioClip newSound = Resources.Load($"Audio/SFX/{name}") as AudioClip;
        if (newSound == null)
        {
            Debug.Log($"Audio File not successfully loaded. The file named {name} likely does not exist in Resources/Audio/SFX");
            return;
        }
        clipStorage.Add(name, newSound);
    }

    // Plays the sound specified by filename
    // First attempts to load the sound through LoadClip()
    public void PlaySFX(AudioContainer sound)
    {
        LoadClip(sound.name);
        if (activeSources >= maxSources)
        {
            Debug.Log("The maximum number  [" + maxSources + "] of AudioSource Components on object [" + this.gameObject + "] has been reached.");
            return;
        }
        else if (clipStorage.ContainsKey(sound.name))
        {
            // create a new AudioSource to use
            activeSources++;
            AudioSource tempSource = this.gameObject.AddComponent<AudioSource>();
            tempSource.volume = sound.volume;
            tempSource.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
            tempSource.maxDistance = sound.maxDistance;
            tempSource.spatialBlend = sound.spatialBlend;
            StartCoroutine(playSoundCoroutine(sound.name, tempSource));
        }
    }

    // Plays the AudioClip from the passed string in the clipStorage
    // Plays the clip through the passed AudioSource, and then deletes the AudioSource
    private IEnumerator playSoundCoroutine(string clipName, AudioSource audioPlayer)
    {
        float timePassed = 0.0f;
        audioPlayer.PlayOneShot(clipStorage[clipName]);

        while (timePassed < clipStorage[clipName].length)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        Destroy(audioPlayer);
        activeSources--;
    }
}