using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // storage of all actively playing Sounds
    [System.NonSerialized] public Dictionary<AudioContainer, int> ActiveSounds;

    // The number of current live AudioSource objects created by this script
    private int activeSources;

    // The maximum number of AudioSource objects allowed to be live at once
    private static int maxSources = 20;

    private void Awake()
    {
        activeSources = 0;
        ActiveSounds = new Dictionary<AudioContainer, int>();
    }

    // Plays the sound specified by filename
    // First attempts to load the sound through LoadClip()
    public void PlaySFX(AudioContainer sound)
    {
        if (activeSources >= maxSources)
        {
            Debug.Log("The maximum number  [" + maxSources + "] of AudioSource Components on object [" + this.gameObject + "] has been reached.");
            return;
        }
        else
        {
            // create a new AudioSource to use
            activeSources++;
            addSound(sound);
            AudioSource tempSource = this.gameObject.AddComponent<AudioSource>();
            tempSource.volume = sound.volume;
            tempSource.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
            tempSource.maxDistance = sound.maxDistance;
            tempSource.spatialBlend = sound.spatialBlend;
            StartCoroutine(playSoundCoroutine(sound, tempSource));
        }
    }

    // Plays the AudioClip from the passed string in the clipStorage
    // Plays the clip through the passed AudioSource, and then deletes the AudioSource
    private IEnumerator playSoundCoroutine(AudioContainer container, AudioSource audioPlayer)
    {
        float timePassed = 0.0f;
        audioPlayer.PlayOneShot(container.clip);

        while (timePassed < container.clip.length)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        Destroy(audioPlayer);
        activeSources--;
        removeSound(container);
    }


    // Adds a counter to the activeSounds storage
    private void addSound(AudioContainer sound)
    {
        if (ActiveSounds.ContainsKey(sound))
        {
            ActiveSounds[sound]++;
        }
        else
        {
            ActiveSounds[sound] = 1;
        }
    }

    // Removes a counter to the activeSounds storage, removes the key if the counter reaches 0
    private void removeSound(AudioContainer sound)
    {
        if (ActiveSounds.ContainsKey(sound))
        {
            ActiveSounds[sound]--;
            if (ActiveSounds[sound] <= 0)
            {
                ActiveSounds.Remove(sound);
            }
        }
    }
}
