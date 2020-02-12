using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public int score { get; private set; } = 0;

    [Header("Sound")]
    [SerializeField] public AudioContainer goldPickup;
    AudioPlayer m_AudioPlayer;


    private void Awake()
    {
        instance = this;
        m_AudioPlayer = this.GetComponent<AudioPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        this.score += score;
        m_AudioPlayer.PlaySFX(goldPickup);
        //m_audioPlayer.playSFX(pointsPlusFileName, pointsPlusVolume, pointsPlusPitchMinimum, pointsPlusPitchMaximum);
    }
}
