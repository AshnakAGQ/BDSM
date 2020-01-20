using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{


    [SerializeField] int numberOfAudioChannels = 5;
    [SerializeField] int soundMaxDistance = 13;
    //[SerializeField] float spatialRatio = 1.0f; Not Used
    [SerializeField] GameObject audioChannelPrefab = null;


    private Dictionary<string, AudioClip> clipStorage = new Dictionary<string,AudioClip>();
    private List<AudioChannel> audioChannels = new List<AudioChannel>();


    void Awake()
    {
        // fill the collection with the specified number of AudioChannels
        audioChannels.Clear();
        for (int i = 0; i < numberOfAudioChannels; i++)
        {
            GameObject temporaryChannelObject = Instantiate(audioChannelPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            temporaryChannelObject.transform.parent = this.transform;
            temporaryChannelObject.transform.localPosition = new Vector3(0, 0, 0);
            AudioChannel temporaryAudioChannel = temporaryChannelObject.GetComponent<AudioChannel>();
            temporaryAudioChannel.inUse = false;
            temporaryAudioChannel.setMaxDistance(soundMaxDistance);

            audioChannels.Add(temporaryAudioChannel);
        }
    }

    // takes a string filename in the path Resources/Audio/SFX
    // loads that audio file into the AudioPlayer's storage
    public void addSFX(string name)
    {
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
        if( ! clipStorage.ContainsKey(name))
        {
            Debug.Log($"The Audio File {name} has not yet been loaded.");
            return;
        }
        else
        {
            for (int i = 0; i < audioChannels.Count; i++)
            {
                if ( ! audioChannels[i].inUse)
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
