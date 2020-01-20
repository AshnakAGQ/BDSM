using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathSound : MonoBehaviour
{

    [SerializeField] string deathFileName = null;
    [SerializeField] float deathVolume = 0.30f;
    [SerializeField] float deathPitchMin = 0.9f;
    [SerializeField] float deathPitchMax = 1.1f;

    AudioPlayer m_audioPlayer;

    private void Awake()
    {
        m_audioPlayer = this.GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(deathFileName);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_audioPlayer.playSFX(deathFileName, deathVolume, deathPitchMin, deathPitchMax);
    }

}
