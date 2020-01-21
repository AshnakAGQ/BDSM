using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField] string musicFileName = null;
    [SerializeField] float volume = 1.0f;
    [SerializeField] float fadeTime = 1.0f;

    public static BGMManager instance { get; private set; }
    AudioSource m_BGMPlayer;

    private void Awake()
    {
        instance = this;
        m_BGMPlayer = GetComponent<AudioSource>();
        setBGMVolume(0.0f);
    }

    private void Start()
    {
        setBGM(musicFileName);
        fadeBGMVolume(volume, fadeTime);
    }

    // sets the background music to the name of the inputted string in Resources/Audio/Music
    public void setBGM(string clip)
    {
        AudioClip newClip = Resources.Load($"Audio/Music/{clip}") as AudioClip;
        m_BGMPlayer.PlayOneShot(newClip);
    }

    // takes a float value between 0 and 1
    // sets the volume of the background music
    public void setBGMVolume(float value)
    {
        m_BGMPlayer.volume = value;
    }

    // fades the volume to end value
    // takes time amount of time to complete
    public void fadeBGMVolume(float finalVolume, float time)
    {
        StartCoroutine(fadeVolumeCoroutine(finalVolume, time));
    }

    private IEnumerator fadeVolumeCoroutine(float finalVolume, float time)
    {
        float startVolume = m_BGMPlayer.volume;
        float compareVolume = startVolume - finalVolume;

        if (startVolume < finalVolume)
        {
            while (m_BGMPlayer.volume - finalVolume <= 0.00001)
            {
                m_BGMPlayer.volume -= compareVolume * Time.deltaTime / time;

                yield return null;
            }
        }

        else
        {
            while (finalVolume - m_BGMPlayer.volume <= 0.00001)
            {
                m_BGMPlayer.volume -= compareVolume * Time.deltaTime / time;

                yield return null;
            }
        }
        
    }
}
