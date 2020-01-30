using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // The maximum distance from the AudioSource object that the sound is able to be heard from
    [SerializeField] int soundMaxDistance = 13;

    // Storage of all AudioClip objects ready to be played
    private Dictionary<string, AudioClip> clipStorage = new Dictionary<string, AudioClip>();

    // takes a string filename in the path Resources/Audio/SFX
    // loads that audio file into the AudioPlayer's storage
    public void loadClip(string name)
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

    // Finds an available/not-in-use AudioChannel and uses it to play the sound specified by filename
    // audio file must be initially loaded by AudioPlayer.addSFX()
    public void playSFX(string name, float soundVolume, float minimumPitch, float maximumPitch)
    {
        if (!clipStorage.ContainsKey(name))
        {
            Debug.Log($"The Audio File {name} has not been loaded.");
            return;
        }
        else
        {
            
            for (int i = 0; i < audioChannels.Count; i++)
            {
                if (!audioChannels[i].inUse)
                {
                    audioChannels[i].playSound(clipStorage[name], soundVolume, minimumPitch, maximumPitch);
                    return;
                }
            }
            Debug.Log("All AudioChannel objects were in use at the time of this function call.");
        }
    }

    public void setSpatialBlend(float ratio)
    {
        for (int i = 0; i < audioChannels.Count; i++)
        {
            audioChannels[i].setSpatialRatio(ratio);
        }
    }

}
